using System.Collections.Generic;
using BaseCode.Logic.Entity;
using BaseCode.Logic.Lights.Services;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Lights.Handler.Abstracts
{
    public abstract class LightBase : MonoBehaviour, ILight
    {
        public LightScriptableObject lightData;
                
        [SerializeField] private LightPlace lightPlace;  // Private field
        [SerializeField] private LightState lightState = (LightState)1;// Private field
        
        private ILightNotifier _notifier;
        private readonly List<VehicleBase> _controlledVehicles = new();

        protected virtual void Start()
        {
            _notifier = new LightNotifier(this);
        }

        public virtual void AddVehicle(VehicleBase vehicle)
        {
            if (_controlledVehicles.Contains(vehicle)) return;
            
            _controlledVehicles.Add(vehicle);
            _notifier.NotifyVehicle(vehicle,CurrentState);
        }

        public virtual void RemoveVehicle(VehicleBase vehicle)
        {
            if (!_controlledVehicles.Contains(vehicle)) return;
            
            _controlledVehicles.Remove(vehicle);
            vehicle.ExitLightControl();
        }

        public abstract void ChangeLight();
        public abstract void SetChangeoverState();

        protected void NotifyStateChange()
        {
            _notifier?.NotifyVehicles(CurrentState);
        }

        public List<VehicleBase> ControlledVehicles => _controlledVehicles;

        public LightPlace Place
        {
            get => lightPlace;
            protected set => lightPlace = value;
        }
        
        public LightState CurrentState
        {
            get => lightState;
            protected set => lightState = value;
        }

    }
}