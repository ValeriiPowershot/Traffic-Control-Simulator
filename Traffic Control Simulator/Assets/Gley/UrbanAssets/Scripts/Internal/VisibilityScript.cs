using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    /// <summary>
    /// Checks if a vehicle is viewed by the camera
    /// </summary>
    public class VisibilityScript : MonoBehaviour
    {
        private bool readyToRemove;
        private bool neverBeenVisible;

        /// <summary>
        /// Check if a vehicle is visible
        /// </summary>
        /// <returns>true it is not in view</returns>
        public bool IsNotInView()
        {
            if (neverBeenVisible == true)
            {
                return true;
            }
            return readyToRemove;
        }


        /// <summary>
        /// Reset component
        /// </summary>
        public void Reset()
        {
            neverBeenVisible = true;
            readyToRemove = false;
        }


        /// <summary>
        /// Unity method automatically triggered
        /// </summary>
        private void OnBecameVisible()
        {
            neverBeenVisible = false;
            readyToRemove = false;
        }


        /// <summary>
        /// Unity method automatically triggered
        /// </summary>
        private void OnBecameInvisible()
        {
            readyToRemove = true;
        }
    }
}
