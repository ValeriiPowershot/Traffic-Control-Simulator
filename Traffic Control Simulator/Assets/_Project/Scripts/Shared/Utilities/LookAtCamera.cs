using UnityEngine;

namespace BaseCode.Utilities
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Camera cam; // Камера (можно не указывать)

        private Transform _cameraTransform;

        private void Awake()
        {
            // Если камера не указана, используем Camera.main
            if (cam == null && Camera.main != null)
                cam = Camera.main;

            if (cam != null)
                _cameraTransform = cam.transform;
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            // Простой Billboard для UI: поворачиваем лицом к камере
            transform.forward = _cameraTransform.forward;
        }
    }
}
