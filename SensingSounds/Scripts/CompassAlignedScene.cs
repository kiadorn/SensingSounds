using GoogleARCore;
using UnityEngine;

namespace CATAHL
{
    /// <summary>
    /// Aligns the position of trackable objects on to copies according to the starting compass value.
    /// </summary>
    public class CompassAlignedScene : MonoBehaviour
    {
        /// <summary>
        /// Singleton.
        /// </summary>
        public static CompassAlignedScene instance;

        /// <summary>
        /// Empty object under the <see cref="CompassAlignedScene"/> parent whose local transform is aligned. Following <see cref="Anchor"/>.
        /// </summary>
        public GameObject alignedAnchor;
        /// Empty object under the <see cref="CompassAlignedScene"/> parent whose local transform is aligned. Following <see cref="LocalPeer"/>.
        public GameObject alignedLocalPeer;

        /// <summary>
        /// <see cref="Anchor"/> transform to follow, set by <see cref="AugmentedImageAnchorController"/>.
        /// </summary>
        public Anchor anchor;
        /// <summary>
        /// <see cref="LocalPeer"/> transform to follow, set in Inspector.
        /// </summary>
        public GameObject localPeer;

        private void Awake()
        {
            Input.compass.enabled = true;
            instance = this;
        }

        private void Start()
        {
            float compassMagnetic = Input.compass.magneticHeading;
            transform.eulerAngles = new Vector3(0, -compassMagnetic, 0);
        }

        private void Update()
        {
            BuildDebug.Log("Start Compass: ", transform.eulerAngles.y);
            UpdateAlignLocalPeer();
            UpdateAlignAnchor();

            BuildDebug.Log("Diff World Peer - Anchor: ", (alignedLocalPeer.transform.position - alignedAnchor.transform.position));
            BuildDebug.Log("Diff Align Peer - Anchor: ", (alignedLocalPeer.transform.localPosition - alignedAnchor.transform.localPosition));
        }

        /// <summary>
        /// Updates the transform of the aligned local peer according to the original local peer.
        /// </summary>
        private void UpdateAlignLocalPeer()
        {
            if (localPeer != null)
            {
                alignedLocalPeer.transform.SetPositionAndRotation(localPeer.transform.position, localPeer.transform.rotation);
                BuildDebug.Log("Local World Pos: ", alignedLocalPeer.transform.position);
                BuildDebug.Log("CliLocalent Align Pos: ", alignedLocalPeer.transform.localPosition);
            }
        }

        /// <summary>
        /// Updates the transform of the aligned anchor according to the original anchor.
        /// </summary>
        private void UpdateAlignAnchor()
        {
            if (anchor != null)
            {
                alignedAnchor.transform.position = anchor.transform.position;
                BuildDebug.Log("Anchor World Pos: ", alignedAnchor.transform.position);
                BuildDebug.Log("Anchor Align Pos: ", alignedAnchor.transform.localPosition);
            }
        }
    }
}
