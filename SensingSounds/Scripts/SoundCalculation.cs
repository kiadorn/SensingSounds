using UnityEngine;
namespace CATAHL
{
    /// <summary>
    /// Calculates properties of the sound played and sets them.
    /// </summary>
    public class SoundCalculation : MonoBehaviour
    {
        //Local variables visible in the inspector.
        [SerializeField]
        [Range(0.01f, 25)]
        private float speed = 1;
        [SerializeField]
        private float minRotationalVolumePercentage = 0.2f;
        [SerializeField]
        private float minRotationalFilterPercentage = 0.2f;
        [SerializeField]
        private AudioLowPassFilter lowPassFilter;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        [Range(0,5000)]
        private float baseFrequencePass = 3000;

        //Local variables
        private float calculatedVolumeTarget;
        private float rotationToTargetPercent;
        private float filterpercentage;
        private float distanceModifier;

        /// <summary>
        /// Gets components from this Gameobject if not assigned in editor.
        /// </summary>
        private void Awake()
        {
            if(audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
            if (lowPassFilter == null)
            {
                lowPassFilter = GetComponent<AudioLowPassFilter>();
            }
        }

        /// <summary>
        /// Updates the volume and soundeffects depending on the angle of the object.
        /// </summary>
        private void Update()
        {
            CalculateAngleToTarget();
            SetVolumeAndEffect();
        }

        /// <summary>
        /// Used to calculate the volume and r effects based on rotation of the <see cref="RemotePeer"/> relative to the <see cref="LocalPeer"/>.
        /// </summary>
        private void CalculateAngleToTarget()
        {
            Vector3 vectorToTarget = (PlayerLocation.GetPlayerLocation() - transform.position);
            float angleToTarget = Vector3.Angle(transform.forward, vectorToTarget);
            rotationToTargetPercent = 1 - (angleToTarget / 180);
            filterpercentage = minRotationalFilterPercentage + ((1 - minRotationalFilterPercentage) * rotationToTargetPercent);
            rotationToTargetPercent = minRotationalVolumePercentage + ((1 - minRotationalVolumePercentage) * rotationToTargetPercent);
        }

        /// <summary>
        /// Sets the volume and effect based on calculated modifiers.
        /// </summary>
        private void SetVolumeAndEffect()
        {
            float volumeModifier = rotationToTargetPercent;
            audioSource.volume = Mathf.Lerp(audioSource.volume, volumeModifier, Time.deltaTime * speed);
            lowPassFilter.cutoffFrequency = filterpercentage * baseFrequencePass;
        }
    }
}
