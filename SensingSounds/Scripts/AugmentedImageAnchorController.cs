using GoogleARCore;
using GoogleARCore.Examples.AugmentedImage;
using System.Collections.Generic;
using UnityEngine;

namespace CATAHL
{
    /// <summary>
    /// Based on GoogleARCore Augmented Image Example Controller.
    /// Sets a world space anchor in the <see cref="CompassAlignedScene"/> after finding an augmented image.
    /// </summary>
    public class AugmentedImageAnchorController : MonoBehaviour
    {
        /// <summary>
        /// List of <see cref="AugmentedImage"/> instances found by <see cref="Session"/>.
        /// </summary>
        private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

        /// <summary>
        /// List of instantiated <see cref="AugmentedImageVisualizer"/>.
        /// </summary>
        private Dictionary<int, AugmentedImageVisualizer> m_Visualizers = new Dictionary<int, AugmentedImageVisualizer>();

        /// <summary>
        /// Prefab of the virtual object placed on top of <see cref="Trackable"/> in scene.
        /// </summary>
        public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

        void Update()
        {
            AugmentImages();
        }

        /// <summary>
        /// Looks for <see cref="Trackable"/> objects by <see cref="Session"/> and instantiates <see cref="AugmentedImageVisualizer"/>.
        /// </summary>
        private void AugmentImages()
        {
            // Get updated augmented images for this frame.
            Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

            // Create visualizers and anchors for updated augmented images that are tracking and do
            // not previously have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempAugmentedImages)
            {
                AugmentedImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
                if (image.TrackingState == TrackingState.Tracking && visualizer == null)
                {
                    // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                    Anchor anchor = image.CreateAnchor(image.CenterPose);

                    CompassAlignedScene.instance.anchor = anchor;

                    visualizer = Instantiate(AugmentedImageVisualizerPrefab, anchor.transform);
                    visualizer.Image = image;
                    m_Visualizers.Add(image.DatabaseIndex, visualizer);
                }
                else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.DatabaseIndex);
                    Destroy(visualizer.gameObject);
                }
            }
        }
    }
}
