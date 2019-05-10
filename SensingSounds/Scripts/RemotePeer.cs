using UnityEngine;

namespace CATAHL
{
    /// <summary>
    /// Class used to represent a peer that connected to a user.
    /// </summary>
    public class RemotePeer : MonoBehaviour
    {
        public TransformPacket transformPacket;

        [SerializeField]
        private AudioSource source;

        /// <summary>
        /// Gets audiosource if not set.
        /// </summary>
        private void Start()
        {
            if(source == null)
            {
                source = GetComponentInChildren<AudioSource>();
            }

        }

        /// <summary>
        /// Sets the position and rotation according to the received packet.
        /// </summary>
        private void Update()
        {
            transform.localPosition = CompassAlignedScene.instance.alignedAnchor.transform.localPosition + transformPacket.position;
            transform.localRotation = transformPacket.rotation;

            BuildDebug.Log("Remote World Pos: ", transform.position);
            BuildDebug.Log("Remote Align Pos: ", transform.localPosition);
            BuildDebug.Log("Remote Received Packet: ", transformPacket.position);
        }

        /// <summary>
        /// Plays a given sound.
        /// </summary>
        /// <param name="number">The number that is converted to the enum value.</param>
        public void PlaySound(int number)
        {
            source.clip = SoundLibrary.GetSound((Sounds)number);
            source.Play();
        }

        public void StopSound()
        {
            source.Stop();
        }
    }
}
