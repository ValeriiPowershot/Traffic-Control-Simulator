using UnityEngine;

namespace BaseCode.Utilities
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Awake()
        {
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (_cameraTransform != null) 
                transform.LookAt(_cameraTransform.position);
        }
    }
}
