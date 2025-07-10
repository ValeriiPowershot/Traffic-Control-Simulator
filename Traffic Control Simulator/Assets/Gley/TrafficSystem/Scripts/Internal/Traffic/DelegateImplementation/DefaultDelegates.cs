using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem
{
    public class DefaultDelegates
    {
        #region VehicleBehaviours
        public static void PlayerInTriggerBehaviour(int vehicleIndex, Collider player)
        {
            API.AddDrivingAction(vehicleIndex, DriveActions.Follow, false);
        }

        public static void DynamicObstacleInTriggerBehaviour(int vehicleIndex, Collider obstacle)
        {
            API.AddDrivingAction(vehicleIndex, DriveActions.StopInDistance, false);
        }


        public static void BuildingInTriggerBehaviour(int vehicleIndex, Collider building)
        {
            API.AddDrivingAction(vehicleIndex, DriveActions.AvoidReverse, false);
        }

        /// <summary>
        /// Called when 2 vehicles hit each other
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="otherIndex"></param>
        /// <param name="addAction">if false resume driving, else check possibilities</param>
        public static void VehicleCrashBehaviour(int vehicleIndex, ObstacleTypes obstacleType, Collider other)
        {
            //Debug.Log("VehicleCrashHandler " + obstacleType, other);

            switch (obstacleType)
            {
                case ObstacleTypes.Player:
                    API.AddDrivingAction(vehicleIndex, DriveActions.StopTemp, false);
                    break;
                case ObstacleTypes.Road:
                    break;
                case ObstacleTypes.TrafficVehicle:
                    API.AddDrivingAction(vehicleIndex, GetCrashAction(vehicleIndex, other), false);
                    break;
                default:
                    API.AddDrivingAction(vehicleIndex, DriveActions.StopTemp, false);
                    break;
            }
        }

        private static VehiclePositioningSystem vehiclePositioningSystem;
        private static TrafficVehicles trafficVehicles;

        private static DriveActions GetCrashAction(int myIndex, Collider other)
        {
#if GLEY_TRAFFIC_SYSTEM
            if(vehiclePositioningSystem == null)
            {
                vehiclePositioningSystem = TrafficManager.Instance.VehiclePositioningSystem;
            }

            if (trafficVehicles == null)
            {
                trafficVehicles = TrafficManager.Instance.TrafficVehicles;
            }
#endif

            int otherIndex = other.attachedRigidbody.GetComponent<VehicleComponent>().GetIndex();
            //determine relative position and moving directions
            int inFront = vehiclePositioningSystem.IsInFront(myIndex, otherIndex);
            bool sameOrientation = vehiclePositioningSystem.IsSameOrientation(trafficVehicles.GetHeading(myIndex), trafficVehicles.GetHeading(otherIndex));
            bool sameHeading = vehiclePositioningSystem.IsSameHeading(trafficVehicles.GetForwardVector(otherIndex), trafficVehicles.GetForwardVector(myIndex));
            bool goingForward = vehiclePositioningSystem.IsGoingForward(trafficVehicles.GetVelocity(myIndex), trafficVehicles.GetHeading(myIndex));

            if (inFront == 2)
            {
                //I am behind
                if (goingForward == true)
                {
                    //I am going forward
                    if (sameOrientation == true)
                    {
                        //the other vehicle is oriented forward
                        if (sameHeading == true)
                        {
                            //other vehicle is going forward
                            //-> I have hit him from behind so I should stop
                            return DriveActions.StopTemp;
                        }
                        else
                        {
                            //if other vehicle is going in reverse i should also
                            return DriveActions.Reverse;
                        }
                    }
                    else
                    {
                        //I am on the wrong way -> reverse
                        return DriveActions.Reverse;
                    }
                }
                else
                {
                    //I am going backwards so I should stop because I hit something
                    return DriveActions.StopTemp;
                }
            }
            else
            {
                if (inFront == 1)
                {
                    //I am in front
                    if (goingForward == true)
                    {
                        // I am going forward
                        if (sameOrientation == false)
                        {
                            // I am on the wrong way and I hit something -> reverse
                            return DriveActions.Reverse;
                        }
                        else
                        {
                            //something hit me from behind, continue
                            return DriveActions.Forward;
                        }
                    }
                    else
                    {
                        //I am going backwards and I hit something -> stop 
                        return DriveActions.StopTemp;
                    }
                }
                else
                {
                    //it is not clear who is in front
                    if (sameHeading)
                    {
                        //if we are going in the same direction I should stop
                        return DriveActions.StopTemp;
                    }
                    else
                    {
                        //I am on the wrong way -> reverse
                        return DriveActions.Reverse;
                    }
                }
            }
        }

        #endregion


        #region SpawnWaypoints
        static GridManager gridManager;


        /// <summary>
        /// The default behavior, a random square is chosen from the available ones 
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static int GetRandomSpawnWaypoint(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType, bool useWaypointPriority)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (gridManager == null)
            {
                gridManager = TrafficManager.Instance.GridManager;
            }

            Vector2Int selectedNeighbor = neighbors[Random.Range(0, neighbors.Count)];

            return GetPossibleWaypoint(selectedNeighbor, vehicleType, useWaypointPriority);
#else
            return -1;
#endif
        }


        /// <summary>
        /// The square in front of the player is chosen
        /// </summary>
        /// <param name="neighbors"></param>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static int GetForwardSpawnWaypoint(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes vehicleType, bool useWaypointPriority)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (gridManager == null)
            {
                gridManager = TrafficManager.Instance.GridManager;
            }

            Vector2Int selectedNeighbor = Vector2Int.zero;
            float angle = 180;
            for (int i = 0; i < neighbors.Count; i++)
            {
                Vector3 cellDirection = gridManager.GetCellPosition(neighbors[i]) - position;
                float newAngle = Vector3.Angle(cellDirection, direction);
                if (newAngle < angle)
                {
                    selectedNeighbor = neighbors[i];
                    angle = newAngle;
                }
            }

            return GetPossibleWaypoint(selectedNeighbor, vehicleType, useWaypointPriority);
#else
            return -1;
#endif
        }

        private static int GetPossibleWaypoint(Vector2Int selectedNeighbor, VehicleTypes vehicleType, bool usePriority)
        {
#if GLEY_TRAFFIC_SYSTEM
            ////get a random waypoint that supports the current vehicle
            List<SpawnWaypoint> possibleWaypoints = gridManager.GetTrafficSpawnWaypointsForCell(selectedNeighbor, vehicleType);
            if (possibleWaypoints.Count > 0)
            {
                if (usePriority)
                {
                    int totalPriority = 0;
                    foreach (SpawnWaypoint waypoint in possibleWaypoints)
                    {
                        totalPriority += waypoint.priority;
                    }
                    int randomPriority = Random.Range(1, totalPriority);
                    totalPriority = 0;
                    for (int i = 0; i < possibleWaypoints.Count; i++)
                    {
                        totalPriority += possibleWaypoints[i].priority;
                        if (totalPriority >= randomPriority)
                        {
                            return possibleWaypoints[i].waypointIndex;
                        }
                    }
                }
                else
                {
                    return possibleWaypoints[Random.Range(0, possibleWaypoints.Count)].waypointIndex;
                }
            }
#endif
            return -1;
        }
        #endregion


        #region TrafficLights
        public static void TrafficLightBehaviour(TrafficLightsColor currentRoadColor, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects, string name)
        {
            switch (currentRoadColor)
            {
                case TrafficLightsColor.Red:
                    SetLight(true, redLightObjects, name);
                    SetLight(false, yellowLightObjects, name);
                    SetLight(false, greenLightObjects, name);
                    break;
                case TrafficLightsColor.YellowRed:
                case TrafficLightsColor.YellowGreen:
                    SetLight(false, redLightObjects, name);
                    SetLight(true, yellowLightObjects, name);
                    SetLight(false, greenLightObjects, name);
                    break;
                case TrafficLightsColor.Green:
                    SetLight(false, redLightObjects, name);
                    SetLight(false, yellowLightObjects, name);
                    SetLight(true, greenLightObjects, name);
                    break;
            }
        }

        /// <summary>
        /// Set traffic lights color
        /// </summary>
        private static void SetLight(bool active, List<GameObject> lightObjects, string name)
        {
            for (int j = 0; j < lightObjects.Count; j++)
            {
                if (lightObjects[j] != null)
                {
                    if (lightObjects[j].activeSelf != active)
                    {
                        lightObjects[j].SetActive(active);
                    }
                }
                else
                {
                    Debug.LogWarning("Intersection " + name + " has null red light objects");
                }
            }
        }
        #endregion


        #region TriggerModifier
        public static void TriggerSizeModifier(float currentSpeed, BoxCollider frontCollider, float maxSpeed, float minTriggerLength, float maxTriggerLength)
        {
            float minSpeed = 20;
            if (currentSpeed < minSpeed)
            {
                frontCollider.size = new Vector3(frontCollider.size.x, frontCollider.size.y, minTriggerLength);
                frontCollider.center = new Vector3(frontCollider.center.x, frontCollider.center.y, minTriggerLength / 2);
            }
            else
            {
                if (currentSpeed >= maxSpeed)
                {
                    frontCollider.size = new Vector3(frontCollider.size.x, frontCollider.size.y, maxTriggerLength);
                    frontCollider.center = new Vector3(frontCollider.center.x, frontCollider.center.y, maxTriggerLength / 2);
                }
                else
                {
                    float newsize = minTriggerLength + (currentSpeed - minSpeed) * ((maxTriggerLength - minTriggerLength) / (maxSpeed - minSpeed));
                    frontCollider.size = new Vector3(frontCollider.size.x, frontCollider.size.y, newsize);
                    frontCollider.center = new Vector3(frontCollider.center.x, frontCollider.center.y, newsize / 2);
                }
            }
        }
        #endregion
    }
}