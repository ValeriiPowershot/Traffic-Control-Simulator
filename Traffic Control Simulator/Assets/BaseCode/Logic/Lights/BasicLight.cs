using System;
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
        [SerializeField] private Light[] _lights;
        [SerializeField] private Sprite[] _lightMats;
        
        [SerializeField] private Image _lightImage;
    
        [SerializeField] private Material[] _groundLightMats;
        [SerializeField] private MeshRenderer _groundMesh;
        [SerializeField] public GameObject light;
        
        private int _currentIndex = 1;
        private const int MaxIndex = 2;

        private void Awake() =>
            UpdateVisualState();

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
                SetActiveLight(_lights.Length - 1);
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

            SetActiveLight(_currentIndex - 1);

            if (_groundMesh != null && _groundLightMats.Length == 2)
            {
                _groundMesh.material = (_currentIndex == 1) ? _groundLightMats[0] : _groundLightMats[1];
            }
        }
        
        public void SetLightInvisibleStatus()
        {
            _groundMesh.enabled = !_groundMesh.isVisible;
            light.SetActive(!light.activeSelf);

            Debug.Log("Light is set to" + light.activeSelf);
        }

        private void SetActiveLight(int i)
        {
            foreach (Light light in _lights)
            {
                light.gameObject.SetActive(false);
            }
            _lights[i].gameObject.SetActive(true);
        }
    }

}
