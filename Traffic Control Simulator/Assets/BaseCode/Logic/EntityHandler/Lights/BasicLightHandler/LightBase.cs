using System.Collections.Generic;
using BaseCode.Domain.Entity;
using BaseCode.Infrastructure.ScriptableObject;
using BaseCode.Logic.EntityHandler.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Lights.BasicLightHandler
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
            vehicle.CarLightService.ExitLightControl();
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