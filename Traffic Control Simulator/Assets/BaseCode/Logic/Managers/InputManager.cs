using BaseCode.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BaseCode.Logic.Managers
{
    public class InputManager : SingletonManagerBase<InputManager>
    {
        [SerializeField] private InputAction _tapAction, _tapPosition;
        [SerializeField] private LayerMask _interactableMask;

        private const float RAYCAST_LENGTH = 500f;

        private bool _tapPerformed;

        private void OnEnable()
        {
            _tapAction.Enable();
            _tapPosition.Enable();
        }
        private void OnDisable()
        {
            _tapAction.Disable();
            _tapPosition.Disable();
        }

        private void Update()
        {
            float PlayerPress = _tapAction.ReadValue<float>();
     
            //if player taps on screen
            if (PlayerPress > 0f && !_tapPerformed)
            {
                _tapPerformed = true;

                Vector2 TapPos = _tapPosition.ReadValue<Vector2>();
                Ray InputRay = CameraManager.Camera.ScreenPointToRay(TapPos);

                //Hits only interactable objects
                if (Physics.Raycast(InputRay, out RaycastHit hit, RAYCAST_LENGTH, _interactableMask))
                {
                    hit.collider.GetComponent<IInteractable>().Interact();
                }
            }
            else if (PlayerPress <= 0f && _tapPerformed)
                _tapPerformed = false;
        }

        public CameraManager CameraManager => GameManager.cameraManager;
    }
}
