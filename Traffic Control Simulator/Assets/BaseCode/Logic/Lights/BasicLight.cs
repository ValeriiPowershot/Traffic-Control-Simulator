using BaseCode.Logic.Lights.Handler.Abstracts;
using UnityEngine;
using UnityEngine.UI;
using LightState = BaseCode.Logic.Vehicles.Vehicles.LightState;

namespace BaseCode.Logic.Lights
{
    public enum LightPlace
    {
        None,
        Forward,
        Left,
        Right,
        Back
    }
    public class BasicLight : LightBase
    {
        [SerializeField] private Sprite[] _lightMats;
        [SerializeField] private Image _lightImage;
    
        [SerializeField] private Material[] _groundLightMats;
        [SerializeField] private MeshRenderer _groundMesh;
        
        private int _currentIndex = 1;
        private const int MaxIndex = 2;

        public override void ChangeLight()
        {
            UpdateLightIndex();
            UpdateVisualState();
            NotifyStateChange();
        }

        private void UpdateLightIndex()
        {
            _currentIndex++;
            if (_currentIndex > MaxIndex) 
                _currentIndex = 1;

            CurrentState = (LightState)_currentIndex;
            Debug.Log(CurrentState);
        }

        public override void SetChangeoverState()
        {
            if (_lightImage != null)
            {
                _lightImage.sprite = _lightMats[^1]; // Set to the last material as a temporary changeover indicator
            }

            if (_groundMesh != null)
            {
                _groundMesh.material = _groundLightMats[^1]; // Set to the last material for the ground as well
            }
        }

        private void UpdateVisualState()
        {
            if (_lightImage != null && _lightMats.Length > 0)
            {
                _lightImage.sprite = _lightMats[_currentIndex - 1];
            }

            if (_groundMesh != null && _groundLightMats.Length == 2)
            {
                // Sync ground light material to the light mesh's first and last material
                if (_currentIndex == 1)
                {
                    _groundMesh.material = _groundLightMats[0]; // First ground material
                }
                else if (_currentIndex == MaxIndex)
                {
                    _groundMesh.material = _groundLightMats[1]; // Last ground material
                }
            }
        }
    }

}
