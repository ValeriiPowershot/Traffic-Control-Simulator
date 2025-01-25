using BaseCode.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BaseCode.Logic
{
    public class InputManager : ManagerBase
    {
        [SerializeField] private InputAction _tapAction, _tapPosition;
        [SerializeField] private LayerMask _interactableMask;

        private const float RAYCAST_LENGTH = 500f;

        private bool _tapPerformed;
        private Camera _cam;

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

        private void Awake()
        {
            _cam = Camera.main;
        }

        void Update()
        {
            float PlayerPress = _tapAction.ReadValue<float>();
     
            //if player taps on screen
            if (PlayerPress > 0f && !_tapPerformed)
            {
                _tapPerformed = true;

                Vector2 TapPos = _tapPosition.ReadValue<Vector2>();
                Ray InputRay = _cam.ScreenPointToRay(TapPos);

                //Hits only interactable objects
                if (Physics.Raycast(InputRay, out RaycastHit hit, RAYCAST_LENGTH, _interactableMask))
                {
                    hit.collider.GetComponent<IInteractable>().Interact();
                }
            }
            else if (PlayerPress <= 0f && _tapPerformed)
                _tapPerformed = false;
        }
    }
}
