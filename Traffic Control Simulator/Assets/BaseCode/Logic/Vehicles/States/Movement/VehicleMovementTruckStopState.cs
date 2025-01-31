using System.Collections;
using BaseCode.Logic.Vehicles.Controllers;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.States.Movement
{
    public class VehicleMovementTruckStopState : IVehicleMovementState
    {
        public VehicleController VehicleController { get; set; }
        public VehicleMovementTruckStopState(VehicleController vehicleController)
        {
            VehicleController = vehicleController;
        }
        public void MovementEnter()
        {
        }

        public void MovementUpdate() 
        {
          
        }
    
        public void MovementExit()
        {
        }
  
         
    }
}