using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Input
{
    public class PinchDetection : MonoBehaviour
    {
        [FormerlySerializedAs("_speed")]
        [SerializeField] private float _cameraSpeed = 4f;
        
        private TouchControl _touchControl;
        private Coroutine _zoomCoroutine;
        private Transform _cameraTransform;

        private void Awake()
        {
            _touchControl = new TouchControl();
            _cameraTransform = Camera.main?.transform;            
        }

        private void OnEnable() =>
            _touchControl.Enable();

        private void OnDisable() =>
            _touchControl.Disable();

        private void Start()
        {
            _touchControl.Touch.SecondaryTouchContact.started += _ => ZoomStart();
            _touchControl.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();
        }

        private void ZoomStart() =>
            _zoomCoroutine = StartCoroutine(ZoomDetection());

        private void ZoomEnd() =>
            StopCoroutine(_zoomCoroutine);

        private IEnumerator ZoomDetection()
        {
            float previousDistance = 0f;
            float currentDistance = 0f;

            while (true)
            {
                currentDistance = Vector2.Distance(
                    _touchControl.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
                    _touchControl.Touch.SecondaryFingerPosition.ReadValue<Vector2>());

                //if(Vector3.Dot(primaryDelta, secondaryDelta) < -.9f)
                //Zoom out
                if (currentDistance > previousDistance)
                {
                    Vector3 targetPosition = _cameraTransform.position;
                    targetPosition.z -=1;
                    _cameraTransform.position = Vector3.Slerp(_cameraTransform.position, targetPosition, Time.deltaTime * _cameraSpeed);
                }
                //Zoom in
                else if (currentDistance < previousDistance)
                {
                    Vector3 targetPosition = _cameraTransform.position;
                    targetPosition.z +=1;
                    Camera.main.orthographicSize--;
                    _cameraTransform.position = Vector3.Slerp
                        (_cameraTransform.position, targetPosition, Time.deltaTime * _cameraSpeed);
                }
                
                previousDistance = currentDistance;
                yield return null;
            }
            
        }
    }
}
