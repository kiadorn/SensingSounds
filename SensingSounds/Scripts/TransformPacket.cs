using LiteNetLib.Utils;
using UnityEngine;
namespace CATAHL
{
    /// <summary>
    /// Packet that contains information about the position and rotation of the local peer.
    /// </summary>
    public struct TransformPacket : INetSerializable
    {
        public Vector3 position;
        public Quaternion rotation;

        public void Deserialize(NetDataReader reader)
        {
            position = reader.GetVector3();
            rotation = reader.GetQuaternion();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(position);
            writer.Put(rotation);
        }
    }
}

