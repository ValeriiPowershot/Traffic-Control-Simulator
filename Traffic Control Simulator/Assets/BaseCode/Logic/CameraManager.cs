using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic
{
    public class CameraManager : SingletonManagerBase<CameraManager>
    {
        public float cameraSpeed = 0.1f;
        public float cameraSizeOnLevelSelection = 0.1f;
        public float cameraSizeOnGame = 0.1f;
        
        private Camera _camera;

        public Camera Camera
        {
            get
            {
                if (_camera == null)
                    _camera = FindObjectOfType<Camera>();

                return _camera;
            }
        }
        public void CameraShake()
        {
            _camera.transform.DOShakePosition(0.5f, 0.2f, 10, 90);
        }
        public void ChangeCameraSizeToLevel()
        {
            if (Camera != null)
                Camera.DOOrthoSize(cameraSizeOnLevelSelection, cameraSpeed);
        }
        public void ChangeCameraSizeToGame()
        {
            if (Camera != null)
                Camera.DOOrthoSize(cameraSizeOnGame, cameraSpeed);
        }

        public void SetNewPosition(Transform currentLevelCameraPosition)
        {
            if (Camera != null)
            {
                Camera.transform.position = currentLevelCameraPosition.position; 
                Camera.transform.rotation = currentLevelCameraPosition.rotation;
            }
        }
    }
}