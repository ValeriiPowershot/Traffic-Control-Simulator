using System.Collections.Generic;
using BaseCode.Logic.EntityHandler.Lights;
using BaseCode.Logic.EntityHandler.Vehicles;
using UnityEngine;

namespace BaseCode.Domain.Entity
{
    public interface ILight
    {
        void ChangeLight();
        void SetChangeoverState();
        void AddVehicle(VehicleBase vehicle);
        void RemoveVehicle(VehicleBase vehicle);
    }
}