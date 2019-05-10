using UnityEngine;

namespace CATAHL {

    /// <summary>
    /// Singleton class to easily get the location of the local player.
    /// </summary>
    public class PlayerLocation : MonoBehaviour {
        public static PlayerLocation singleton;

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        private void Awake() {
            if (singleton != null) {
                Destroy(singleton.gameObject);
            }
            singleton = this;
        }

        /// <summary>
        /// Used to get the local players position.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetPlayerLocation() {
            return singleton.transform.position;
        }
    }
}