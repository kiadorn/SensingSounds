using UnityEngine;
using LiteNetLib.Utils;

namespace CATAHL
{
    /// <summary>
    /// Enables <see cref="Vector3"/> and <see cref="Quaternion"/> to be sent in packages easily.
    /// Has methods that extend the <see cref="NetDataReader"/> and <see cref="NetDataWriter"/> classes.
    /// </summary>
    public static class PacketProcessorExtension
    {
        /// <summary>
        /// Puts each element of the <see cref="Vector3"/> in the package.
        /// </summary>
        /// <param name="writer">The writer of the package.</param>
        /// <param name="vector">The vector to send.</param>
        public static void Put(this NetDataWriter writer, Vector3 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
            writer.Put(vector.z);
        }

        /// <summary>
        /// Reads elements from the package and constructs a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="reader">The reader of the package.</param>
        /// <returns>Returns the new vector.</returns>
        public static Vector3 GetVector3(this NetDataReader reader)
        {
            Vector3 v;
            v.x = reader.GetFloat();
            v.y = reader.GetFloat();
            v.z = reader.GetFloat();
            return v;
        }

        /// <summary>
        /// Puts each element of the <see cref="Quaternion"/> in the package.
        /// </summary>
        /// <param name="writer">The writer of the package.</param>
        /// <param name="quat">The quaternion to base the package on.</param>
        public static void Put(this NetDataWriter writer, Quaternion quat)
        {
            writer.Put(quat.x);
            writer.Put(quat.y);
            writer.Put(quat.z);
            writer.Put(quat.w);
        }

        /// <summary>
        /// Reads elements from the package and constructs a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="reader">The reader to get the quaternion elements from.</param>
        /// <returns>Returns the new quaternion.</returns>
        public static Quaternion GetQuaternion(this NetDataReader reader)
        {
            Quaternion q;
            q.x = reader.GetFloat();
            q.y = reader.GetFloat();
            q.z = reader.GetFloat();
            q.w = reader.GetFloat();
            return q;
        }
    }
}

