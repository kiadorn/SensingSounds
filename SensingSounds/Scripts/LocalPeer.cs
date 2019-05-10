using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace CATAHL
{
    /// <summary>
    /// Class to represent the user.
    /// Connects to peers, sends and recieves packets.
    /// </summary>
    public class LocalPeer : MonoBehaviour, INetEventListener
    {
        private NetManager manager;
        private NetDataWriter writer;
        private NetPacketProcessor packetProcessor;

        /// <summary>
        /// The port to connect to.
        /// </summary>
        [SerializeField]
        private int port = 8888;

        /// <summary>
        /// The message used to validate a discovery request.
        /// </summary>
        [SerializeField]
        private string discoveryMessage = "DISCOVERY";

        private IPEndPoint localIP;

        private Dictionary<NetPeer, RemotePeer> otherPlayers = new Dictionary<NetPeer, RemotePeer>();

        /// <summary>
        /// Prefab of <see cref="RemotePeer"/>.
        /// </summary>
        [SerializeField]
        private GameObject remotePeerPrefab = null;

        private float discoveryTimer = 0f;

        private TransformPacket transformPacket;         

        /// <summary>
        /// Sets initial states and runs setup on required components.
        /// </summary>
        private void Start()
        {
            packetProcessor = new NetPacketProcessor();
            packetProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector3());
            packetProcessor.RegisterNestedType<TransformPacket>();

            Input.gyro.enabled = true;
            Input.compensateSensors = true;

            writer = new NetDataWriter();
            writer.Reset();
            writer.Put(discoveryMessage);

            NetManagerSetup();
            FetchLocalIP();
            SendAndResetDiscoveryRequest();
        }

        /// <summary>
        /// Sends packets to peers.
        /// Takes events regarding incoming packets and handles them.
        /// </summary>
        private void Update()
        {
            manager.PollEvents();

            SetPacketRotation();
            SetPacketPosition();

            BuildDebug.Log("Raw Vector: ", Input.compass.rawVector);
            BuildDebug.Log("Gyro: ", Input.gyro.attitude.eulerAngles);
            BuildDebug.Log("Compass magnetic: ", Input.compass.magneticHeading);
            BuildDebug.Log("Compass true: ", Input.compass.trueHeading);
            BuildDebug.Log("Compass accuracy: ", Input.compass.headingAccuracy);

            var peer = manager.FirstPeer;
            if (peer == null || peer.ConnectionState != ConnectionState.Connected)
            {
                discoveryTimer += Time.deltaTime;

                if (discoveryTimer > 2f)
                {
                    SendAndResetDiscoveryRequest();
                }
                return;
            }

            SendPacketSerializable(PacketType.Transform, transformPacket, DeliveryMethod.Unreliable);
        }

        /// <summary>
        /// Stops the manager when exiting.
        /// </summary>
        private void OnApplicationQuit()
        {
            manager.Stop();
        }

        /// <summary>
        /// Stops the manager when client is removed for any reason.
        /// </summary>
        private void OnDestroy()
        {
            manager.Stop();
        }

        /// <summary>
        /// General method to send a serialized packet.
        /// </summary>
        /// <typeparam name="T">General type of packet.</typeparam>
        /// <param name="type">packet typ</param>
        /// <param name="packet">The packet to send.</param>
        /// <param name="deliveryMethod">Deliverymethod.</param>
        public void SendPacketSerializable<T>(PacketType type, T packet, DeliveryMethod deliveryMethod) where T : INetSerializable
        {
            writer.Reset();
            writer.Put((byte)type);
            packet.Serialize(writer);
            manager.SendToAll(writer, deliveryMethod);
        }

        /// <summary>
        /// Sends a packet containing an integer.
        /// </summary>
        /// <param name="number">The integer sent.</param>
        /// <param name="type">packettype.</param>
        /// <param name="deliveryMethod">Type of delivery.</param>
        public void SendIntPacket(int number, PacketType type, DeliveryMethod deliveryMethod)
        {
            writer.Reset();
            writer.Put((byte)type);
            writer.Put(number);
            manager.SendToAll(writer, deliveryMethod);
        }

        public void SendBytePacket(PacketType type, DeliveryMethod deliveryMethod)
        {
            writer.Reset();
            writer.Put((byte)type);
            manager.SendToAll(writer, deliveryMethod);
        }

        #region INetEventListener Implementation
        /// <summary>
        /// Accepts a connection request.
        /// </summary>
        /// <param name="request">The request send.</param>
        public void OnConnectionRequest(ConnectionRequest request)
        {
            NetPeer requestedPeer = request.Accept();
        }

        //Not used.
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        //Not used.
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        /// <summary>
        /// Event that runs when a packet is revieved from a peer.
        /// </summary>
        /// <param name="peer">The peer that send a packet.</param>
        /// <param name="reader">The reader to get the packet from.</param>
        /// <param name="deliveryMethod">The type of delivery.</param>
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            byte packetTypeByte = reader.GetByte();
            PacketType packetType = (PacketType)packetTypeByte;

            switch (packetType)
            {
                case PacketType.Transform:
                    otherPlayers[peer].transformPacket.Deserialize(reader);
                    break;
                case PacketType.Sounds:
                    otherPlayers[peer].PlaySound(reader.GetInt());
                    break;
                case PacketType.Stop:
                    otherPlayers[peer].StopSound();
                    break;
                default:
                    Debug.LogError("Unhandled packet: " + packetType);
                    break;
            }
            reader.Clear();
        }

        /// <summary>
        /// Handles connectionmessages sent from a peer.
        /// </summary>
        /// <param name="remoteEndPoint">Information about the connection.</param>
        /// <param name="reader">Reader of netpacket.</param>
        /// <param name="messageType">What type of message is sent.</param>
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {

            if (remoteEndPoint.Address.ToString() != localIP.Address.ToString())
            {
                if (messageType == UnconnectedMessageType.DiscoveryRequest)
                {
                    manager.SendDiscoveryResponse(writer, remoteEndPoint);
                }
                else if (messageType == UnconnectedMessageType.DiscoveryResponse)
                {
                    manager.Connect(remoteEndPoint, writer);
                }
            }
        }

        /// <summary>
        /// Event that happends when connecting to a peer.
        /// Adds the peer to the local system.
        /// </summary>
        /// <param name="peer">The peer connected to.</param>
        public void OnPeerConnected(NetPeer peer)
        {
            manager.ConnectedPeerList.Add(peer);
            AddClient(peer);
        }

        /// <summary>
        /// Event for when a peer disconnects.
        /// Removes properties of that peer.
        /// </summary>
        /// <param name="peer">The previously connected peer.</param>
        /// <param name="disconnectInfo">Info about the disconnect.</param>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("Disconnected Peer: " + disconnectInfo.Reason.ToString() + " >:(");
            manager.ConnectedPeerList.Remove(peer);
            RemoveClient(peer);
            if (manager.ConnectedPeerList.Count == 0)
            {
                writer.Reset();
                writer.Put(discoveryMessage);
            }
        }
        #endregion

        /// <summary>
        /// Sets the local IP.
        /// </summary>
        private void FetchLocalIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                localIP = socket.LocalEndPoint as IPEndPoint;
            }
        }

        /// <summary>
        /// Sends discover requests if not connected to anyone.
        /// </summary>
        private void SendAndResetDiscoveryRequest()
        {
            if (manager.SendDiscoveryRequest(writer, port))
            {
                discoveryTimer = 0f;
            }
            else
            {
                Debug.LogError("Failed Discovery Request on port: " + port);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void NetManagerSetup()
        {
            manager = new NetManager(this);
            manager.Stop();
            manager.AutoRecycle = true;
            manager.DiscoveryEnabled = true;
            manager.Start(port);
        }
        /// <summary>
        /// Used to add a peer.
        /// </summary>
        /// <param name="peer">Peer to add.</param>
        private void AddClient(NetPeer peer)
        {
            GameObject newPlayer = Instantiate(remotePeerPrefab, CompassAlignedScene.instance.transform);
            newPlayer.name = "Remote Client " + peer.EndPoint.ToString();
            RemotePeer newRemoteClient = newPlayer.GetComponentInChildren<RemotePeer>();
            otherPlayers.Add(peer, newRemoteClient);
        }

        /// <summary>
        /// Used to remove a peer.
        /// </summary>
        /// <param name="peer">The peer to remove.</param>
        private void RemoveClient(NetPeer peer)
        {
            Destroy(otherPlayers[peer].transform.gameObject);
            otherPlayers.Remove(peer);
        }

        /// <summary>
        /// Sends a sound to peers.
        /// </summary>
        /// <param name="number">The type of sound to be sent.</param>
        public void SendSound(int number = 1)
        {
            SendIntPacket(number, PacketType.Sounds, DeliveryMethod.ReliableOrdered);
            SoundLibrary.SoundSent(number);
        }

        /// <summary>
        /// Stops playing the current sound.
        /// </summary>
        public void StopSound()
        {
            SendBytePacket(PacketType.Stop, DeliveryMethod.ReliableOrdered);
            SoundLibrary.StopSound();
        }

        /// <summary>
        /// Sets the rotation to send to peers.
        /// </summary>
        private void SetPacketRotation()
        {
            if (SystemInfo.supportsGyroscope)
            {
                transformPacket.rotation = CompassAlignedScene.instance.alignedLocalPeer.transform.localRotation;
            }
            else
            {
                transformPacket.rotation = transform.localRotation;
            }
        }

        /// <summary>
        /// Sets the position to send in the packet, calculates the positional difference from the anchor to the client and sends.
        /// </summary>
        private void SetPacketPosition()
        {
            Vector3 localDiff = CompassAlignedScene.instance.alignedLocalPeer.transform.localPosition - CompassAlignedScene.instance.alignedAnchor.transform.localPosition;
            transformPacket.position = localDiff;
            BuildDebug.Log("Sent Packet: ", transformPacket.position);
        }
    }
}