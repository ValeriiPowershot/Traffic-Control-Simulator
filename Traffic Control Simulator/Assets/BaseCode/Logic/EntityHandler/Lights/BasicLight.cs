using System.Collections.Generic;
using BaseCode.Domain.Entity;
using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler.Lights.BasicLightHandler;
using BaseCode.Logic.EntityHandler.Vehicles;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.EntityHandler.Lights
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
        [SerializeField] private Material[] lightMats;
        [SerializeField] private MeshRenderer lightMesh;
        
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
            if (_currentIndex > MaxIndex) _currentIndex = 1;

            CurrentState = (LightState)_currentIndex;
        }

        public override void SetChangeoverState()
        {
            if (lightMesh != null)
            {
                lightMesh.material = lightMats[^1]; // Set to the last material as a temporary changeover indicator
            }
        }

        private void UpdateVisualState()
        {
            if (lightMesh != null && lightMats.Length > 0)
            {
                lightMesh.material = lightMats[_currentIndex - 1];
            }
        }
    }
}

/*public class LightManager : MonoBehaviour
{
    [SerializeField] private List<VehicleBase> vehicles; // Populate via Inspector
    [SerializeField] private BasicLight basicLight;

    private void Start()
    {
        var notifier = new LightNotifier(vehicles);
        basicLight = new BasicLight(notifier)
        {
            Place = LightPlace.Forward // Set the light place dynamically if needed
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) // Example for testing light change
        {
            basicLight.ChangeLight();
        }
    }
}*/
