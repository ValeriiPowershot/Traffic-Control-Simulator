using System;
using BaseCode.Logic;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Ways;
using Script.ScriptableObject;
using Script.Vehicles.Controllers;
using Script.Vehicles.States;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Script.Vehicles
{
    public class Vehicle : BasicCar
    {
        [SerializeField] private VehicleController _vehicleController;

        public AllWaysContainer AllWaysContainer;
        public VehicleScriptableObject VehicleScriptableObject;
        public Transform RayStartPoint;

        private CarManager _manager;
        private WaypointContainer _wayPointContainer;

        public WaypointContainer WaypointContainer { get { return _wayPointContainer; } }

        // this will be called by spawn manager
        public void Starter(CarManager Manager, AllWaysContainer Container)
        {
            AllWaysContainer = Container;
            _manager = Manager;
            _vehicleController.Starter(this);
        }
        
        public void Update() =>
            _vehicleController.Update();

        public override void PassLightState(LightState state, LightPlace lightPlace)
        {
            Debug.Log("Passing light state to vehicle " + state);
            base.PassLightState(state,lightPlace);

            switch (state)
            {
                case LightState.Green:
                    _vehicleController.SetState<VehicleGoState>();
                    break;
                case LightState.Red:
                    _vehicleController.SetState<VehicleStopState>();
                    break;
                case LightState.Yellow:
                    _vehicleController.SetState<VehicleSlowDownState>();
                    break;
                case LightState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        public void InitializePath()
        {
            _wayPointContainer = AllWaysContainer.AllWays[0];
            float rangeToCurr = (_wayPointContainer.transform.position - transform.position).sqrMagnitude;
            
            for (int i = 0; i < AllWaysContainer.AllWays.Length; i++)
            {
                float rangeToNext = (AllWaysContainer.AllWays[i].transform.position - transform.position).sqrMagnitude;

                if (rangeToNext < rangeToCurr)
                {
                    _wayPointContainer = AllWaysContainer.AllWays[i];
                    rangeToCurr = (_wayPointContainer.transform.position - transform.position).sqrMagnitude;
                }
                else if (rangeToNext > rangeToCurr)
                    continue;
                else
                {
                    _wayPointContainer = Random.Range(0, 2) == 0 ? _wayPointContainer : AllWaysContainer.AllWays[i];
                    rangeToCurr = (_wayPointContainer.transform.position - transform.position).sqrMagnitude;
                }
            }
            _vehicleController.StateController.InitializePath();
        }

        public void DestinationReached()
        {
            _manager.CarDestinationReched(this);
        }
    }
}