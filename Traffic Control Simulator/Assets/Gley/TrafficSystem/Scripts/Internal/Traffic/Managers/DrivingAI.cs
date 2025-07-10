using System.Collections.Generic;
using UnityEngine;
using Gley.UrbanAssets.Internal;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Decides what the vehicle will do next based on received information
    /// </summary>
    public class DrivingAI : MonoBehaviour
    {
        private WaypointManager waypointManager;
        private TrafficVehicles trafficVehicles;
        private VehiclePositioningSystem vehiclePositioningSystem;
        private PositionValidator positionValidator;
        private DriveActions[] driveActions;
        private BlinkType[] blinkTypes;
        private PlayerInTrigger playerInTrigger;
        private DynamicObstacleInTrigger dynamicObstacleInTrigger;
        private BuildingInTrigger buildingInTrigger;
        private VehicleCrash vehicleCrash;
        private float[] waypointSpeed;
        private bool actionsDebug;
        private bool speedDebug;
        private bool debugPathFinding;

        //all available vehicle actions
        TrafficSystem.DriveActions[] currentActiveAction;
        TrafficSystem.DriveActions[] movingActions = new TrafficSystem.DriveActions[]
        {
            TrafficSystem.DriveActions.AvoidForward,
            TrafficSystem.DriveActions.AvoidReverse,
            TrafficSystem.DriveActions.Reverse,
            TrafficSystem.DriveActions.StopInDistance,
            TrafficSystem.DriveActions.Follow,
            TrafficSystem.DriveActions.Overtake
        };


        /// <summary>
        /// Initialize Driving AI
        /// </summary>
        /// <param name="nrOfVehicles"></param>
        /// <param name="waypointManager"></param>
        /// <param name="trafficVehicles"></param>
        /// <param name="vehiclePositioningSystem"></param>
        /// <param name="actionsDebug"></param>
        /// <param name="speedDebug"></param>
        /// <param name="aiDebug"></param>
        /// <returns></returns>
        public DrivingAI Initialize(int nrOfVehicles, WaypointManager waypointManager, TrafficVehicles trafficVehicles, VehiclePositioningSystem vehiclePositioningSystem, PositionValidator positionValidator, bool actionsDebug, bool speedDebug, bool debugPathFinding, 
            PlayerInTrigger playerInTrigger,DynamicObstacleInTrigger dynamicObstacleInTrigger, BuildingInTrigger buildingInTrigger, VehicleCrash vehicleCrash)
        {
            this.waypointManager = waypointManager;
            this.trafficVehicles = trafficVehicles;
            this.vehiclePositioningSystem = vehiclePositioningSystem;
            this.positionValidator = positionValidator;
            this.actionsDebug = actionsDebug;
            this.speedDebug = speedDebug;
            this.debugPathFinding = debugPathFinding;

            driveActions = new DriveActions[nrOfVehicles];
            waypointSpeed = new float[nrOfVehicles];
            blinkTypes = new BlinkType[nrOfVehicles];
            currentActiveAction = new TrafficSystem.DriveActions[nrOfVehicles];

            this.playerInTrigger = playerInTrigger;
            this.dynamicObstacleInTrigger = dynamicObstacleInTrigger;
            this.buildingInTrigger = buildingInTrigger;
            SetVehicleCrashDelegate(vehicleCrash);

            for (int i = 0; i < nrOfVehicles; i++)
            {
                driveActions[i].activeActions = new List<DriveAction>();
            }

            //triggered every time a new object is seen by the front trigger
            VehicleEvents.onObjectInTrigger += ObjectInTriggerHandler;
            //triggered every time there are no objects left in trigger
            VehicleEvents.onTriggerCleared += TriggerClearedHandler;

            WaypointEvents.onStopStateChanged += StopStateChangedHandler;
            WaypointEvents.onGiveWayStateChanged += GiveWayStateChangedHandler;

            return this;
        }


        private void TriggerClearedHandler(int vehicleIndex)
        {
            RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.Continue);
        }


        internal void SetPlayerInTriggerDelegate(PlayerInTrigger newDelegate)
        {
            playerInTrigger = newDelegate;
        }


        internal void SetDynamicObstacleInTriggerDelegate(DynamicObstacleInTrigger newDelegate)
        {
            dynamicObstacleInTrigger = newDelegate;
        }


        internal void SetBuildingInTriggerDelegate(BuildingInTrigger newDelegate)
        {
            buildingInTrigger = newDelegate;
        }


        internal void SetVehicleCrashDelegate(VehicleCrash newDelegate)
        {
            Events.onVehicleCrashed -= vehicleCrash;
            vehicleCrash = newDelegate;
            Events.onVehicleCrashed += vehicleCrash;
        }

        /// <summary>
        /// Reset all pending actions, used when a vehicle is respawned
        /// </summary>
        /// <param name="index"></param>
        public void VehicleActivated(int index)
        {
            waypointSpeed[index] = waypointManager.GetMaxSpeed(index);
            SetBlinkType(index, BlinkType.Stop, true);
            AIEvents.TriggetChangeDrivingStateEvent(index, currentActiveAction[index], GetActionValue(currentActiveAction[index], index));
        }


        public void RemoveVehicle(int index)
        {
            driveActions[index].activeActions = new List<DriveAction>();
            currentActiveAction[index] = TrafficSystem.DriveActions.Forward;
        }


        /// <summary>
        /// Based on position, heading, and speed decide what is the next action of the vehicle
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="otherIndex"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        TrafficSystem.DriveActions GetTriggerAction(int myIndex, Collider other)
        {
            VehicleComponent otherVehicle = other.attachedRigidbody.GetComponent<VehicleComponent>();
            int otherIndex = otherVehicle.GetIndex();
            //if it already in other vehicle trigger, stop
            if (otherVehicle.AlreadyCollidingWith(trafficVehicles.GetAllColliders(myIndex)))
            {
                if (otherVehicle.GetCurrentAction() != TrafficSystem.DriveActions.StopTemp && otherVehicle.GetCurrentAction() != TrafficSystem.DriveActions.StopInDistance)
                {
                    return TrafficSystem.DriveActions.StopTemp;
                }
                else
                {
                    return TrafficSystem.DriveActions.ForceForward;
                }
            }

            //if reverse is true, means that other car is reversing so I have to reverse
            //if (reverse)
            //{
            //    return SpecialDriveActionTypes.Reverse;
            //}

            bool sameOrientation = vehiclePositioningSystem.IsSameOrientation(trafficVehicles.GetHeading(myIndex), trafficVehicles.GetHeading(otherIndex));

            //if other car is stationary
            if (trafficVehicles.GetCurrentAction(otherIndex) == TrafficSystem.DriveActions.StopInDistance || trafficVehicles.GetCurrentAction(otherIndex) == TrafficSystem.DriveActions.StopInPoint || trafficVehicles.GetCurrentAction(otherIndex) == TrafficSystem.DriveActions.GiveWay)
            {
                if (sameOrientation)
                {
                    //if the orientation is the same I stop too
                    return TrafficSystem.DriveActions.StopInDistance;
                }
            }
            else
            {
                bool sameHeading = vehiclePositioningSystem.IsSameHeading(trafficVehicles.GetForwardVector(otherIndex), trafficVehicles.GetForwardVector(myIndex));
                bool otherIsGoingForward = vehiclePositioningSystem.IsGoingForward(trafficVehicles.GetVelocity(otherIndex), trafficVehicles.GetHeading(otherIndex));


                if (sameOrientation == false && sameHeading == false)
                {
                    //not same orientation -> going in opposite direction-> try to avoid it
                    return TrafficSystem.DriveActions.AvoidForward;
                }
                else
                {
                    //same orientation but different moving direction 
                    if (otherIsGoingForward == false)
                    {
                        // other car is going in reverse so I should also
                        return TrafficSystem.DriveActions.Reverse;
                    }
                }

                if (sameHeading == false)
                {
                    //going back and hit something -> wait
                    return TrafficSystem.DriveActions.StopTemp;
                }
                else
                {
                    //follow the car in front
                    if (trafficVehicles.GetVelocity(myIndex).sqrMagnitude > 5 && trafficVehicles.GetVelocity(otherIndex).sqrMagnitude > 5)
                    {
                        //if the relative angle between the 2 cars is small enough -> follow
                        if (Mathf.Abs(Vector3.SignedAngle(trafficVehicles.GetForwardVector(otherIndex), trafficVehicles.GetForwardVector(myIndex), Vector3.up)) < 35)
                        {
                            return TrafficSystem.DriveActions.Follow;
                        }
                    }
                }
                //if nothing worked, stop in distance
                return TrafficSystem.DriveActions.StopInDistance;
            }
            //continue forward
            return TrafficSystem.DriveActions.Forward;
        }


        private void ObjectInTriggerHandler(int vehicleIndex, ObstacleTypes obstacleType, Collider other)
        {
            switch (obstacleType)
            {
                case ObstacleTypes.TrafficVehicle:
                    AddDriveAction(vehicleIndex, GetTriggerAction(vehicleIndex, other));
                    break;
                case ObstacleTypes.Player:
                    PlayerInTrigger(vehicleIndex, other);
                    break;
                case ObstacleTypes.DynamicObject:
                    DynamicObjectInTrigger(vehicleIndex, other);
                    break;
                case ObstacleTypes.StaticObject:
                    BuildingObjectInTrigger(vehicleIndex, other);
                    break;
            }
        }


        void PlayerInTrigger(int vehicleIndex, Collider other)
        {
            playerInTrigger?.Invoke(vehicleIndex, other);
        }


        void DynamicObjectInTrigger(int vehicleIndex, Collider other)
        {
            dynamicObstacleInTrigger?.Invoke(vehicleIndex, other);
        }


        void BuildingObjectInTrigger(int vehicleIndex, Collider other)
        {
            buildingInTrigger?.Invoke(vehicleIndex, other);
        }


        internal void ChangeLane(bool active, int vehicleIndex, RoadSide side)
        {
            if (active)
            {
                AddDriveAction(vehicleIndex, TrafficSystem.DriveActions.ChangeLane, false, side);
            }
            else
            {
                RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.ChangeLane);
            }
        }


        internal void AddDriveAction(int index, TrafficSystem.DriveActions newAction, bool force = false, RoadSide side = RoadSide.Any)
        {
            if (newAction == TrafficSystem.DriveActions.ForceForward)
            {
                force = true;
                newAction = TrafficSystem.DriveActions.Forward;
            }

            if (force)
            {
                driveActions[index].activeActions.Clear();
            }

            //if the new action is not already in the list-> add it in the required position based on priority
            if (!driveActions[index].Contains(newAction))
            {
                bool added = false;
                for (int i = 0; i < driveActions[index].activeActions.Count; i++)
                {
                    if (driveActions[index].activeActions[i].GetActionType() < newAction)
                    {
                        driveActions[index].activeActions.Insert(i, new DriveAction(newAction, side));
                        added = true;
                        break;
                    }
                }
                if (added == false)
                {
                    driveActions[index].Add(new DriveAction(newAction, side));
                }
                ApplyAction(index);
            }

        }


        internal void RemoveDriveAction(int index, TrafficSystem.DriveActions newAction)
        {
            //car is out of trigger -> remove current action
            if (newAction == TrafficSystem.DriveActions.Continue)
            {
                // remove all active actions
                driveActions[index].RemoveAll(movingActions);
            }
            else
            {
                //remove just current action
                driveActions[index].Remove(newAction);
            }
            ApplyAction(index);
        }


        /// <summary>
        /// Apply the first action from list
        /// </summary>
        /// <param name="index"></param>
        void ApplyAction(int index)
        {
            //if trigger is true, other vehicles needs to be alerted that the current action changed
            bool trigger = false;
            if (driveActions[index].activeActions.Count == 0)
            {
                //if list is empty, go forward by default 
                currentActiveAction[index] = TrafficSystem.DriveActions.Forward;
                trigger = true;
            }
            else
            {
                if (currentActiveAction[index] != driveActions[index].activeActions[0].GetActionType())
                {
                    trigger = true;
                    currentActiveAction[index] = driveActions[index].activeActions[0].GetActionType();
                }
            }

            //if (currentActiveAction[index] != SpecialDriveActionTypes.Follow && currentActiveAction[index] != SpecialDriveActionTypes.Overtake)
            //{
            //    //reset follow speed if no longer follow a vehicle
            //    Debug.Log("RESET FOLLWO SPEED " + index);
            //    vehicleToFollow[index] = -1;
            AIEvents.TriggerChangeDestinationEvent(index);
            //}

            if (trigger)
            {
                //trigger corresponding events based on new action

                switch (currentActiveAction[index])
                {
                    case TrafficSystem.DriveActions.Reverse:
                    case TrafficSystem.DriveActions.AvoidReverse:
                    case TrafficSystem.DriveActions.StopInDistance:
                    case TrafficSystem.DriveActions.StopInPoint:
                    case TrafficSystem.DriveActions.GiveWay:
                        AIEvents.TriggerNotifyVehiclesEvent(index, trafficVehicles.GetCollider(index));
                        break;
                }
                AIEvents.TriggetChangeDrivingStateEvent(index, currentActiveAction[index], GetActionValue(currentActiveAction[index], index));
            }
        }


        /// <summary>
        /// Returns execution times for each action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        float GetActionValue(TrafficSystem.DriveActions action, int index)
        {
            switch (action)
            {
                case TrafficSystem.DriveActions.Reverse:
                    return 5;
                case TrafficSystem.DriveActions.StopTemp:
                    return Random.Range(3, 5);
                case TrafficSystem.DriveActions.Follow:
                    return 1;
                default:
                    return Mathf.Infinity;
            }
        }


        internal void TimedActionEnded(int index)
        {
            switch (currentActiveAction[index])
            {
                case TrafficSystem.DriveActions.Follow:
                    AddDriveAction(index, TrafficSystem.DriveActions.Overtake);
                    break;
                case TrafficSystem.DriveActions.Reverse:
                case TrafficSystem.DriveActions.StopTemp:
                case TrafficSystem.DriveActions.AvoidReverse:
                    RemoveDriveAction(index, currentActiveAction[index]);
                    trafficVehicles.CurrentVehicleActionDone(index);
                    break;
                default:
                    RemoveDriveAction(index, currentActiveAction[index]);
                    break;
            }
        }


        /// <summary>
        /// Called when a waypoint state changed to update the current vehicle actions
        /// </summary>
        /// <param name="index"></param>
        /// <param name="stopState"></param>
        /// <param name="giveWayState"></param>
        private void StopStateChangedHandler(int index, bool stopState)
        {
            if (stopState == true)
            {
                AddDriveAction(index, TrafficSystem.DriveActions.StopInPoint);
            }
            else
            {
                RemoveDriveAction(index, TrafficSystem.DriveActions.StopInPoint);
            }

        }


        private void GiveWayStateChangedHandler(int index, bool giveWayState)
        {
            if (giveWayState)
            {
                AddDriveAction(index, TrafficSystem.DriveActions.GiveWay);
            }
            else
            {
                RemoveDriveAction(index, TrafficSystem.DriveActions.GiveWay);
            }

        }


        /// <summary>
        /// Called when a vehicle needs a new waypoint
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="carType"></param>
        public void WaypointRequested(int vehicleIndex, VehicleTypes carType, bool clearPath)
        {
            int freeWaypointIndex;
            for (int i = 0; i < driveActions[vehicleIndex].activeActions.Count; i++)
            {
                TrafficSystem.DriveActions activeAction = driveActions[vehicleIndex].activeActions[i].GetActionType();

                switch (activeAction)
                {
                    case TrafficSystem.DriveActions.StopInPoint:
                        //if current action is stop in point -> no new waypoint is needed
                        if (waypointManager.HasPath(vehicleIndex))
                        {
                            if (waypointManager.GetPath(vehicleIndex).Count == 0)
                            {
                                Events.TriggerDestinationReachedEvent(vehicleIndex);
                                RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.StopInPoint);
                            }
                        }
                        return;

                    case TrafficSystem.DriveActions.ChangeLane:
                        //if the current vehicle can overtake
                        if (driveActions[vehicleIndex].activeActions[i].GetSide() == RoadSide.Any)
                        {
                            freeWaypointIndex = waypointManager.GetOtherLaneWaypointIndex(vehicleIndex, (int)carType);
                        }
                        else
                        {
                            freeWaypointIndex = waypointManager.GetOtherLaneWaypointIndex(vehicleIndex, (int)carType, driveActions[vehicleIndex].activeActions[i].GetSide(), trafficVehicles.GetForwardVector(vehicleIndex));
                        }

                        if (freeWaypointIndex == -1)
                        {
                            //if cannot change lane
                            ContinueStraight(vehicleIndex, carType);
                            if (clearPath)
                            {
                                if (!waypointManager.GetTargetWaypointOfAgent<WaypointBase>(vehicleIndex).name.Contains("Connect"))
                                {
                                    trafficVehicles.SetMaxSpeed(vehicleIndex, trafficVehicles.GetMaxSpeed(vehicleIndex) * 0.7f);
                                }
                                else
                                {
                                    trafficVehicles.ResetMaxSpeed(vehicleIndex);
                                }
                            }
                        }
                        else
                        {
                            if (clearPath)
                            {
                                trafficVehicles.ResetMaxSpeed(vehicleIndex);
                            }
                            Blink(BlinkReasons.Overtake, vehicleIndex, freeWaypointIndex);
                            //can overtake, make sure path is free
                            if (AllClear(vehicleIndex, freeWaypointIndex, clearPath))
                            {
                                if (!clearPath)
                                {
                                    RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.ChangeLane);
                                }
                                SetNextWaypoint(vehicleIndex, freeWaypointIndex);
                            }
                            else
                            {
                                ContinueStraight(vehicleIndex, carType);
                            }
                        }
                        return;

                    case TrafficSystem.DriveActions.Overtake:
                        //if the current vehicle can overtake
                        freeWaypointIndex = waypointManager.GetOtherLaneWaypointIndex(vehicleIndex, (int)carType);
                        if (freeWaypointIndex == -1)
                        {
                            //if cannot change lane
                            ContinueStraight(vehicleIndex, carType);
                        }
                        else
                        {
                            Blink(BlinkReasons.Overtake, vehicleIndex, freeWaypointIndex);
                            //can overtake, make sure path is free
                            if (AllClear(vehicleIndex, freeWaypointIndex, false))
                            {
                                //if can change lane -> start blinking
                                SetNextWaypoint(vehicleIndex, freeWaypointIndex);
                            }
                            else
                            {
                                ContinueStraight(vehicleIndex, carType);
                            }
                        }
                        return;

                    case TrafficSystem.DriveActions.GiveWay:
                        if (waypointManager.IsInIntersection(vehicleIndex))
                        {
                            if (waypointManager.CanEnterIntersection(vehicleIndex))
                            {
                                freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(vehicleIndex, (int)carType);
                                if (freeWaypointIndex != -1)
                                {
                                    RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.GiveWay);
                                    Blink(BlinkReasons.ChangeLane, vehicleIndex, freeWaypointIndex);
                                    SetNextWaypoint(vehicleIndex, freeWaypointIndex);
                                }
                            }
                        }
                        else
                        {
                            Waypoint waypoint = waypointManager.GetTargetWaypointOfAgent<Waypoint>(vehicleIndex);
                            if (waypoint.complexGiveWay)
                            {
                                freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(vehicleIndex, (int)carType);
                                if (freeWaypointIndex != -1)
                                {
                                    Blink(BlinkReasons.GiveWay, vehicleIndex, freeWaypointIndex);
                                    if (!waypointManager.AreTheseWaypointsATarget(waypoint.giveWayList))
                                    {
                                        RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.GiveWay);
                                        SetNextWaypoint(vehicleIndex, freeWaypointIndex);
                                    }
                                }
                                return;
                            }

                            freeWaypointIndex = waypointManager.GetOtherLaneWaypointIndex(vehicleIndex, (int)carType);
                            if (freeWaypointIndex == -1)
                            {
                                freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(vehicleIndex, (int)carType);
                            }


                            if (freeWaypointIndex != -1)
                            {
                                Blink(BlinkReasons.GiveWay, vehicleIndex, freeWaypointIndex);
                                Waypoint nextWaypoint = waypointManager.GetWaypoint<Waypoint>(freeWaypointIndex);
                                if (nextWaypoint.zipperGiveWay)
                                {
                                    if (!waypointManager.IsThisWaypointATarget(freeWaypointIndex))
                                    {
                                        RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.GiveWay);
                                        SetNextWaypoint(vehicleIndex, freeWaypointIndex);
                                    }
                                    return;
                                }

                                if (AllClear(vehicleIndex, freeWaypointIndex, clearPath))
                                {
                                    if (waypoint.neighbors.Count > 0)
                                    {
                                        RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.GiveWay);
                                    }
                                    SetNextWaypoint(vehicleIndex, freeWaypointIndex);
                                }
                            }
                            else
                            {
                                Blink(BlinkReasons.NoWaypoint, vehicleIndex, freeWaypointIndex);
                                AddDriveAction(vehicleIndex, TrafficSystem.DriveActions.NoWaypoint);
                            }

                        }
                        //If current vehicle has to give way -> wait until new waypoint is free
                        return;

                }
            }
            //if current vehicle is in no special state -> set next waypoint without any special requirements
            freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(vehicleIndex, (int)carType);

            if (freeWaypointIndex >= 0)
            {
                Blink(BlinkReasons.None, vehicleIndex, freeWaypointIndex);
                SetNextWaypoint(vehicleIndex, freeWaypointIndex);

                if (!waypointManager.CanContinueStraight(vehicleIndex, (int)carType))
                {
                    AddDriveAction(vehicleIndex, TrafficSystem.DriveActions.GiveWay);
                }

                //remove the no waypoint action if waypoints are found -> used for temporary disable waypoints
                if (driveActions[vehicleIndex].activeActions.Count > 0)
                {
                    if (driveActions[vehicleIndex].activeActions[0].GetActionType() == TrafficSystem.DriveActions.NoWaypoint)
                    {
                        RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.NoWaypoint);
                    }

                    if (driveActions[vehicleIndex].activeActions[0].GetActionType() == TrafficSystem.DriveActions.NoPath)
                    {
                        RemoveDriveAction(vehicleIndex, TrafficSystem.DriveActions.NoPath);
                    }
                }
            }
            else
            {
                NoWaypointsAvailable(vehicleIndex, freeWaypointIndex);
            }
        }


        private bool AllClear(int vehicleIndex, int freeWaypointIndex, bool clearPath)
        {
            //get the average speed of the car
            float maxWaypointSpeed = waypointManager.GetWaypoint<Waypoint>(freeWaypointIndex).maxSpeed;
            float maxCarSpeed = Mathf.Min(maxWaypointSpeed, trafficVehicles.GetMaxSpeed(vehicleIndex));
            //average between current speed and max speed
            float averageSpeed = (trafficVehicles.GetCurrentSpeed(vehicleIndex) + maxCarSpeed) / 2 / 3.6f;

            //calculate the distance to the next waypoint 
            float distance = Vector3.Distance(waypointManager.GetTargetWaypointOfAgent<Waypoint>(vehicleIndex).position, waypointManager.GetWaypoint<Waypoint>(freeWaypointIndex).position);

            //time it takes for the car to reach next waypoint
            float time = distance / averageSpeed;
            //distance needed to be free on the road
            float distanceToCheck = maxWaypointSpeed * time;

            if (clearPath)
            {
                distanceToCheck *= 0.1f;
            }

            int incomingCarIndex = -1;
            //if everything is free -> can go
            if (waypointManager.AllPreviousWaypointsAreFree(vehicleIndex, distanceToCheck, freeWaypointIndex, ref incomingCarIndex))
            {
                return true;
            }
            else
            {
                //check speed
                if (incomingCarIndex != -1)
                {
                    if (trafficVehicles.GetCurrentSpeed(incomingCarIndex) < 1)
                    {
                        if (!waypointManager.IsThisWaypointATarget(freeWaypointIndex))
                        {
                            VehicleComponent vehicle = trafficVehicles.GetVehicleComponent(vehicleIndex);
                            if (positionValidator.IsPositionFree(waypointManager.GetWaypointPosition(freeWaypointIndex), vehicle.length, vehicle.coliderHeight, vehicle.colliderWidth, waypointManager.GetNextOrientation(freeWaypointIndex)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        void ContinueStraight(int vehicleIndex, VehicleTypes carType)
        {
            //get new waypoint on the same lane
            int freeWaypointIndex = waypointManager.GetCurrentLaneWaypointIndex(vehicleIndex, (int)carType);
            if (freeWaypointIndex >= 0)
            {
                SetNextWaypoint(vehicleIndex, freeWaypointIndex);
            }
            else
            {
                Blink(BlinkReasons.NoWaypoint, vehicleIndex, freeWaypointIndex);
                NoWaypointsAvailable(vehicleIndex, freeWaypointIndex);
            }
        }


        void NoWaypointsAvailable(int vehicleIndex, int waypointIndex)
        {
            if (waypointIndex == -2)
            {
                AddDriveAction(vehicleIndex, TrafficSystem.DriveActions.NoPath);
                Events.TriggerDestinationReachedEvent(vehicleIndex);
                return;
            }

            AddDriveAction(vehicleIndex, TrafficSystem.DriveActions.NoWaypoint);
        }


        void SetNextWaypoint(int index, int freeWaypointIndex)
        {
            //Debug.Log(freeWaypointIndex);
            waypointManager.SetNextWaypoint(index, freeWaypointIndex);
            waypointSpeed[index] = waypointManager.GetMaxSpeed(index);
            AIEvents.TriggerChangeDestinationEvent(index);
        }

        #region Blinking
        public void SetBlinkType(int vehicleIndex, BlinkType value, bool reset = false)
        {
            if (reset)
            {
                blinkTypes[vehicleIndex] = BlinkType.Stop;
            }
            if (value == BlinkType.Stop && blinkTypes[vehicleIndex] == BlinkType.Hazard)
            {
                return;
            }
            if (blinkTypes[vehicleIndex] != value)
            {

                blinkTypes[vehicleIndex] = value;
                trafficVehicles.SetBlinkLights(vehicleIndex, value);
            }
        }

        internal void SetHazardLights(int vehicleIndex, bool activate)
        {
            if (activate)
            {
                SetBlinkType(vehicleIndex, BlinkType.Hazard);
            }
            else
            {
                SetBlinkType(vehicleIndex, BlinkType.Stop, true);
            }
        }

        /// <summary>
        /// Determine if blink is required
        /// </summary>
        /// <param name="possibleLaneChange"></param>
        /// <param name="index"></param>
        /// <param name="oldWaypoint"></param>
        /// <param name="newWaypoint"></param>
        /// <param name="oldPoz"></param>
        /// <param name="forward"></param>
        private void Blink(BlinkReasons blinkReason, int index, int newWaypointindex)
        {
            if(blinkReason == BlinkReasons.NoWaypoint)
            {
                SetBlinkType(index, BlinkType.Hazard);
                return;
            }

            int oldWaypointIndex = waypointManager.GetTargetWaypointIndex(index);
            Vector3 forward = trafficVehicles.GetForwardVector(index);
            Waypoint oldWaypoint = waypointManager.GetWaypoint<Waypoint>(oldWaypointIndex);
            Waypoint newWaypoint = waypointManager.GetWaypoint<Waypoint>(newWaypointindex);
            Waypoint targetWaypoint = newWaypoint;
            if (blinkReason == BlinkReasons.None)
            {
                if (oldWaypoint.neighbors.Count > 1)
                {
                    blinkReason = BlinkReasons.ChangeLane;
                }
            }

            switch (blinkReason)
            {
                case BlinkReasons.Overtake:
                case BlinkReasons.GiveWay:
                    float angle = Vector3.SignedAngle(forward, newWaypoint.position - oldWaypoint.position, Vector3.up);
                    SetBlinkType(index, DetermineBlinkDirection(angle));
                    break;

                case BlinkReasons.ChangeLane:
                    for (int i = 0; i < 5; i++)
                    {
                        if (targetWaypoint.neighbors.Count > 0)
                        {
                            targetWaypoint = waypointManager.GetWaypoint<Waypoint>(targetWaypoint.neighbors[0]);
                        }
                    }
                    angle = Vector3.SignedAngle(oldWaypoint.position - waypointManager.GetWaypoint<Waypoint>(oldWaypoint.prev[0]).position, targetWaypoint.position - oldWaypoint.position, Vector3.up);
                    SetBlinkType(index, DetermineBlinkDirection(angle));
                    break;

                case BlinkReasons.None:
                    if (newWaypoint.neighbors.Count > 0)
                    {
                        targetWaypoint = waypointManager.GetWaypoint<Waypoint>(targetWaypoint.neighbors[0]);
                        angle = Vector3.SignedAngle(oldWaypoint.position - newWaypoint.position, oldWaypoint.position - targetWaypoint.position, Vector3.up);
                        if (Mathf.Abs(angle) < 1)
                        {
                            SetBlinkType(index, BlinkType.Stop);
                        }
                    }
                    break;

                case BlinkReasons.NoWaypoint:
                    SetBlinkType(index, BlinkType.Hazard);
                    break;
            }
        }




        /// <summary>
        /// Determine the blink direction
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="index"></param>
        private BlinkType DetermineBlinkDirection(float angle)
        {
            if (angle > 5)
            {
                return BlinkType.BlinkRight;
            }

            if (angle < -5)
            {
                return BlinkType.BlinkLeft;
            }
            return BlinkType.Stop;
        }
        #endregion

        /// <summary>
        /// Compute current maximum available speed in m/s
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetMaxSpeedMS(int index)
        {
            return ComputeMaxPossibleSpeed(index) / 3.6f;
        }


        float ComputeMaxPossibleSpeed(int index)
        {
            float maxSpeed;

            if (currentActiveAction[index] == TrafficSystem.DriveActions.Follow || currentActiveAction[index] == TrafficSystem.DriveActions.Overtake)
            {
                maxSpeed = Mathf.Min(trafficVehicles.GetFollowSpeed(index) * 3.6f, trafficVehicles.GetMaxSpeed(index), waypointSpeed[index]);
            }
            else
            {
                maxSpeed = Mathf.Min(trafficVehicles.GetMaxSpeed(index), waypointSpeed[index]);
            }

            return maxSpeed;
        }


        /// <summary>
        /// Events cleanup
        /// </summary>
        private void OnDestroy()
        {
            Events.onVehicleCrashed -= vehicleCrash;
            VehicleEvents.onTriggerCleared -= TriggerClearedHandler;
            WaypointEvents.onStopStateChanged -= StopStateChangedHandler;
            WaypointEvents.onGiveWayStateChanged -= GiveWayStateChangedHandler;
            VehicleEvents.onObjectInTrigger -= ObjectInTriggerHandler;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (actionsDebug)
            {
                List<VehicleComponent> allVehicles = trafficVehicles.GetVehicleList();
                for (int i = 0; i < allVehicles.Count; i++)
                {
                    string hasPath = waypointManager.HasPath(allVehicles[i].GetIndex()) ? "Has Path":"";
                    string text = $"{allVehicles[i].GetIndex()}. Action {allVehicles[i].GetCurrentAction()} {hasPath} \n";
                    if (speedDebug)
                    {
                        text += "Current Speed " + allVehicles[i].GetCurrentSpeed().ToString("N1") + "\n" +
                        "Follow Speed " + trafficVehicles.GetFollowSpeed(allVehicles[i].GetIndex()).ToString("N1") + "\n" +
                        "Waypoint Speed " + waypointSpeed[allVehicles[i].GetIndex()].ToString("N1") + "\n" +
                        "Max Speed" + trafficVehicles.GetMaxSpeed(allVehicles[i].GetIndex()).ToString("N1") + "\n";
                    }

                    Handles.Label(allVehicles[i].transform.position + new Vector3(1, 1, 1), text);
                    if (debugPathFinding)
                    {
                        if(waypointManager.HasPath(allVehicles[i].GetIndex()))
                        { 
                            Queue<int> path = waypointManager.GetPath(allVehicles[i].GetIndex());
                            foreach (int n in path)
                            {
                                Gizmos.color = Color.red;
                                Vector3 position = CurrentSceneData.GetSceneInstance().GetComponent<PathFindingData>().allPathFindingWaypoints[n].worldPosition;
                                Gizmos.DrawWireSphere(position, 1);
                                position.y += 1;
                                UnityEditor.Handles.Label(position, allVehicles[i].GetIndex().ToString());
                            }
                        }
                    }
                }
            }         
        }
#endif
    }
}


