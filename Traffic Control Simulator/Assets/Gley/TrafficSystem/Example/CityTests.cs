using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Internal;
using UnityEngine;

namespace Gley.TrafficSystem.Example
{
    public class CityTests : MonoBehaviour
    {
        [SerializeField] Transform busStops;
        private bool pathSet;
        private int stopNumber;

        //every time a destination is reached, a new one is selected
        private void BusStationReached(int vehicleIndex)
        {
            //remove listener otherwise this method will be called on each frame
            Events.onDestinationReached -= BusStationReached;
            if (vehicleIndex == 0)
            {
                stopNumber++;
                if (stopNumber == busStops.childCount)
                {
                    stopNumber = 0;
                }
                //stop and wait for 5 seconds, then move to the next destination
                API.AddDrivingAction(0, TrafficSystem.DriveActions.Stop, true);
                Invoke("ContinueDriving", 5);
            }
        }

        /// <summary>
        /// Continue on path
        /// </summary>
        private void ContinueDriving()
        {
            Events.onDestinationReached += BusStationReached;
            API.AddDrivingAction(0, TrafficSystem.DriveActions.Forward, true);
            API.SetDestination(0, busStops.GetChild(stopNumber).transform.position);
        }

        private void Update()
        {
            if (!pathSet)
            {
                if (API.IsInitialized())
                {
                    if (CurrentSceneData.GetSceneInstance().GetComponent<PathFindingData>() != null)
                    {
                        pathSet = true;
                        SetPath();
                    }
                }
            }

            //if 1 key is press the vehicles will move towards the edge of the road
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                API.ClearPathForSpecialVehicles(true, RoadSide.Right);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                API.ClearPathForSpecialVehicles(false, RoadSide.Right);
            }

            if(UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// set a path towards destination
        /// </summary>
        private void SetPath()
        {
            VehicleComponent vehicleComponent = API.GetVehicleComponent(0);
            if (vehicleComponent.gameObject.activeSelf)
            {
                Events.onDestinationReached += BusStationReached;
                API.SetDestination(0, busStops.GetChild(stopNumber).transform.position);
            }
            else
            {
                Invoke("SetPath", 1);
            }
        }

        //remove listeners
        private void OnDestroy()
        {
            Events.onDestinationReached -= BusStationReached;
        }
    }
}