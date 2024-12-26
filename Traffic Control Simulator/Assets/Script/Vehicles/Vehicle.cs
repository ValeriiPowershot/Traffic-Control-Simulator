using System;
using BaseCode.Logic;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Ways;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace Script.Vehicles
{
    public class Vehicle : BasicCar
    {
        [SerializeField] private VehicleController vehicleController;

        public AllWaysContainer allWaysContainer;
        public Transform rayStartPoint;
        
        public WaypointContainer WaypointContainer { get; set; }
        public VehicleScriptableObject VehicleScriptableObject { get; set; }

        public void Starter(CarManager Manager, AllWaysContainer Container, VehicleScriptableObject currentCar)
        {
            allWaysContainer = Container;
            CarManager = Manager;
            VehicleScriptableObject = currentCar;
            
            vehicleController.Starter(this);
        }
        
        public void Update() => vehicleController.Update();

        public override void PassLightState(LightState state, LightPlace lightPlace)
        {
            base.PassLightState(state,lightPlace);

            switch (state)
            {
                case LightState.Green:
                    vehicleController.SetState<VehicleGoState>();
                    break;
                case LightState.Red:
                    //vehicleController.SetState<VehicleStopState>();
                    break;
                case LightState.Yellow:
                    vehicleController.SetState<VehicleSlowDownState>();
                    break;
                case LightState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        public void AssignNewPathContainer()
        {
            vehicleController.StateController.InitializePath();

            //the WaypointContainer is assigned in car manager and this code 
            //was performing the same work second time and been causing the car spawn bug
            
            /*WaypointContainer = allWaysContainer.AllWays[0];
            float rangeToCurr = GetSquaredDistance(WaypointContainer.transform.position);

            foreach (var wayPointContainer in allWaysContainer.AllWays)
            {
                float rangeToNext = GetSquaredDistance(wayPointContainer.transform.position);
                if (rangeToNext < rangeToCurr)
                {
                    WaypointContainer = wayPointContainer;
                    rangeToCurr = GetSquaredDistance(WaypointContainer.transform.position);
                }
                else if (rangeToNext > rangeToCurr)
                    continue;
                else
                {
                    WaypointContainer = Random.value < 0.5f ? WaypointContainer : wayPointContainer;
                    rangeToCurr = (WaypointContainer.transform.position - transform.position).sqrMagnitude;
                }
            }
            vehicleController.StateController.InitializePath();*/
        }

        private float GetSquaredDistance(Vector3 targetPosition)
        {
            return (targetPosition - transform.position).sqrMagnitude;
        }

        public void DestinationReached()
        {
            CarManager.CarDestinationReached(this);
        }
    }
}