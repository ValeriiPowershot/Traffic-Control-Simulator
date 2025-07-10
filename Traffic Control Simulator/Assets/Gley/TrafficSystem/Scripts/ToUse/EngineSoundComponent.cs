using UnityEngine;

namespace Gley.TrafficSystem
{
    /// <summary>
    /// Add this component on the Vehicle prefab if you need engine sound for your vehicle
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class EngineSoundComponent : MonoBehaviour
    {
        [Tooltip("Pitch used when vehicle is stationary")]
        public float minPitch = 0.6f;
        [Tooltip("Pitch used when vehicle is at max speed")]
        public float maxPitch = 1;
        [Tooltip("Volume used when vehicle is stationary")]
        public float minVolume = 0.5f;
        [Tooltip("Volume used when vehicle is at max speed")]
        public float maxVolume = 1;

        private AudioSource audioSource;


        /// <summary>
        /// Initialize the sound component if required
        /// </summary>
        public void Initialize()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;      
        }


        /// <summary>
        /// Play sound is vehicle is enabled
        /// </summary>
        /// <param name="masterVolume"></param>
        public void Play(float masterVolume)
        {
            audioSource.volume = masterVolume;
            audioSource.Play();
        }


        /// <summary>
        /// Stop volume when vehicle is disabled
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
        }


        /// <summary>
        /// Update engine sound based on speed
        /// </summary>
        /// <param name="velocity">current vehicle speed</param>
        /// <param name="maxVelocity">max vehicle speed</param>
        /// <param name="masterVolume">master volume</param>
        public void UpdateEngineSound(float velocity, float maxVelocity, float masterVolume)
        {
            float percent = velocity / maxVelocity;
            audioSource.volume = (minVolume + (maxVolume - minVolume) * percent) * masterVolume;

            float pitch = minPitch + (maxPitch - minPitch) * percent;
            audioSource.pitch = pitch;
        }
    }
}