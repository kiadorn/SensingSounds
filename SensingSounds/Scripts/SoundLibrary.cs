using UnityEngine;
namespace CATAHL{ 

    /// <summary>
    /// Struct used to set attributes of a sound.
    /// </summary>
    [System.Serializable]
    public struct AudioClipWithType {
        public Sounds sound;
        public string name;
        public AudioClip audioClip;
    }

    /// <summary>
    /// Soundlibrary to get soundclips, can be used with different parameters.
    /// Also used to toggle availablity of buttons.
    /// </summary>
    public class SoundLibrary : MonoBehaviour
    {
        //public variables.
        public AudioClipWithType[] availableSounds;
        public CanvasGroup buttonGroup;
        public GameObject soundSent;

        //Singleton
        private static SoundLibrary singleton;

        /// <summary>
        /// Used for the singleton pattern.
        /// </summary>
        private void Awake() {
            if(singleton != null) {
                Destroy(singleton.gameObject);
            }
            singleton = this;
        }

        /// <summary>
        /// Used to get a specific audioclip. Uses a enum as parameter.
        /// </summary>
        /// <param name="sound">Enum of the type of sound requested.</param>
        /// <returns>Returns audioclip if such is available.</returns>
        public static AudioClip GetSound(Sounds sound) {
            foreach (AudioClipWithType clip in singleton.availableSounds) {
                if (clip.sound.Equals(sound)) {
                    return clip.audioClip;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a sound based on arrayindex.
        /// </summary>
        /// <param name="soundIndex">The index from the array of available sounds.</param>
        /// <returns>Returns audioclip if such is available.</returns>
        public static AudioClip GetSound(int soundIndex) {
            return singleton.availableSounds[soundIndex].audioClip;
        }

        /// <summary>
        /// Returns a clip based on the name of it in the struct <see cref="AudioClipWithType"/>.
        /// </summary>
        /// <param name="soundName">The name of the clip.</param>
        /// <returns>Returns audioclip if such is available.</returns>
        public static AudioClip GetSound(string soundName) {
            foreach(AudioClipWithType clip in singleton.availableSounds) {
                if (clip.name.Equals(soundName)) {
                    return clip.audioClip;
                }
            }
            return null;
        }

        /// <summary>
        /// Used when sounds are sent. Changes the availability of the buttons so that they are not available when sounds are playing.
        /// </summary>
        /// <param name="sound">The sound sent.</param>
        public static void SoundSent(int sound)
        {
            singleton.ChangeButtons(false);
            singleton.Invoke("ChangeButtonsInvoke", GetSound((Sounds)sound).length);
        }

        public static void StopSound()
        {
            singleton.CancelInvoke();
            singleton.ChangeButtons(true);
        }

        /// <summary>
        /// Used to delay turning buttons on.
        /// </summary>
        public void ChangeButtonsInvoke()
        {
            ChangeButtons(true);
        }

        /// <summary>
        /// General method to change the status of the soundbuttons.
        /// </summary>
        /// <param name="activateStatus"></param>
        public void ChangeButtons(bool activateStatus)
        {
            singleton.soundSent.SetActive(!activateStatus);
            singleton.buttonGroup.interactable = activateStatus;
        }
    }
}