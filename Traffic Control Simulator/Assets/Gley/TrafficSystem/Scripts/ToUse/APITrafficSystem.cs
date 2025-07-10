using Gley.TrafficSystem.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gley.TrafficSystem
{
    public static class API
    {
        #region TrafficManager
        /// <summary>
        /// Initialize the traffic system
        /// </summary>
        /// <param name="activeCamera">Camera that follows the player or the player itself.</param>
        /// <param name="nrOfVehicles">Maximum number of traffic vehicles active at the same time.</param>
        /// <param name="vehiclePool">Available vehicles asset.</param>
        public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool vehiclePool)
        {
            Initialize(activeCamera, nrOfVehicles, vehiclePool, new TrafficOptions());
        }


        /// <summary>
        /// Initialize the traffic system
        /// </summary>
        /// <param name="activeCamera">Camera that follows the player or the player itself.</param>
        /// <param name="nrOfVehicles">Maximum number of traffic vehicles active at the same time.</param>
        /// <param name="vehiclePool">Available vehicles asset.</param>
        /// <param name="trafficOptions">An object used to store the initialization parameters.</param>
        public static void Initialize(Transform activeCamera, int nrOfVehicles, VehiclePool vehiclePool, TrafficOptions trafficOptions)
        {
            Initialize(new Transform[] { activeCamera }, nrOfVehicles, vehiclePool, trafficOptions);
        }


        /// <summary>
        /// Initialize the traffic system
        /// </summary>
        /// <param name="activeCameras">Camera that follows the player or the player itself.</param>
        /// <param name="nrOfVehicles">Maximum number of traffic vehicles active at the same time.</param>
        /// <param name="vehiclePool">Available vehicles asset.</param>
        /// <param name="trafficOptions">An object used to store the initialization parameters.</param>
        public static void Initialize(Transform[] activeCameras, int nrOfVehicles, VehiclePool vehiclePool, TrafficOptions trafficOptions)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.Initialize(activeCameras, nrOfVehicles, vehiclePool, trafficOptions);
#endif
        }


        /// <summary>
        /// Check if the Traffic System is initialized
        /// </summary>
        /// <returns>true if initialized</returns>
        public static bool IsInitialized()
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.IsInitialized();
#else
            return false;
#endif

        }


        /// <summary>
        /// Update the active camera that is used to remove vehicles when are not in view
        /// </summary>
        /// <param name="activeCamera">Represents the camera or the player prefab</param>
        public static void SetCamera(Transform activeCamera)
        {
            SetCameras(new Transform[] { activeCamera });
        }


        /// <summary>
        /// Update active cameras that are used to remove vehicles when are not in view
        /// this is used in multiplayer/split screen setups
        /// </summary>
        /// <param name="activeCameras">Represents the cameras or the players from your game</param>
        public static void SetCameras(Transform[] activeCameras)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.UpdateCamera(activeCameras);
#endif
        }


        /// <summary>
        /// All traffic vehicles in the scene will decelerate and change lanes towards the edge of the road to create space for special vehicles such as police or ambulances. 
        /// </summary>
        /// <param name="active">If true vehicle will drive on the side of the road.</param>
        /// <param name="side">Specifies the road side.</param>
        public static void ClearPathForSpecialVehicles(bool active, RoadSide side)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.ClearPathForSpecialVehicles(active, side);
#endif
        }


        /// <summary>
        /// When this method is called, the vehicle passed as param is no longer controlled by the traffic system 
        /// until it is out of view and respawned
        /// </summary>
        /// <param name="vehicle">The vehicle to be removed from the Traffic System.</param>
        public static void StopVehicleDriving(GameObject vehicle)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.StopVehicleDriving(vehicle);
#endif
        }


        /// <summary>
        /// Adds a vehicle and sets a predefined path to destination
        /// </summary>
        /// <param name="position">A Vector3 for the initial position. The vehicle will be placed on the closest waypoint from this position.</param>
        /// <param name="vehicleType">The type of the vehicle to be instantiated.</param>
        /// <param name="destination">A Vector3 for the destination position. The closest waypoint from this position will be the destination of the vehicle.</param>
        public static void AddVehicleWithPath(Vector3 position, VehicleTypes vehicleType, Vector3 destination)
        {
            AddVehicleWithPath(position, vehicleType, destination, null);
        }


        /// <summary>
        /// Adds a vehicle and sets a predefined path to destination
        /// </summary>
        /// <param name="position">A Vector3 for the initial position. The vehicle will be placed on the closest waypoint from this position.</param>
        /// <param name="vehicleType">The type of the vehicle to be instantiated.</param>
        /// <param name="destination">A Vector3 for the destination position. The closest waypoint from this position will be the destination of the vehicle.</param>
        /// <param name="completeMethod">Callback triggered after initialization. It returns the VehicleComponent and the waypoint index where the vehicle was instantiated.</param>
        public static void AddVehicleWithPath(Vector3 position, VehicleTypes vehicleType, Vector3 destination, UnityAction<VehicleComponent, int> completeMethod)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.AddVehicleWithPath(position, vehicleType, destination, completeMethod);
#endif
        }


        /// <summary>
        /// Remove a specific vehicle from scene
        /// </summary>
        /// <param name="vehicle">Root GameObject of the vehicle to remove</param>
        public static void RemoveVehicle(GameObject vehicle)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.RemoveVehicle(vehicle);
#endif
        }


        /// <summary>
        /// Remove a specific vehicle from scene
        /// </summary>
        /// <param name="vehicleIndex">Index of the vehicle to remove</param>
        public static void RemoveVehicle(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.RemoveVehicle(vehicleIndex, true);
#endif
        }


        /// <summary>
        /// Removes all the vehicles from a given area.
        /// </summary>
        /// <param name="center">The center of the circle to remove vehicles from.</param>
        /// <param name="radius">The radius in meters of the circle to remove vehicles from.</param>
        public static void ClearTrafficOnArea(Vector3 center, float radius)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.ClearTrafficOnArea(center, radius);
#endif
        }


        /// <summary>
        /// Set how far away active intersections should be -> default is 1
        /// If set to 2 -> intersections will update on a 2 square distance from the player
        /// </summary>
        /// <param name="level">How many squares away should intersections be updated</param>
        public static void SetActiveSquares(int level)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.SetActiveSquaresLevel(level);
#endif
        }
        #endregion


        #region Density
        /// <summary>
        /// Modify max number of active vehicles
        /// </summary>
        /// <param name="nrOfVehicles">New max number of vehicles, needs to be less than the initialization max number of vehicles</param>
        public static void SetTrafficDensity(int nrOfVehicles)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DensityManager?.SetTrafficDensity(nrOfVehicles);
#endif
        }


        /// <summary>
        /// Disable all waypoints on the specified area to stop vehicles to go in a certain area for a limited amount of time
        /// </summary>
        /// <param name="center">The center of the circle to disable waypoints from.</param>
        /// <param name="radius">The radius in meters of the circle to disable waypoints from.</param>
        public static void DisableAreaWaypoints(Vector3 center, float radius)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DensityManager?.DisableAreaWaypoints(new Area(center, radius));
#endif
        }


        /// <summary>
        /// This will instantiate an excluded vehicle, the vehicle will work normally, but when it is removed it will not be instantiated again
        /// If the index sent as parameter is not an excluded vehicle, it will be ignored
        /// Call AddExcludedVehicleToSystem to make it behave normally
        /// </summary>
        /// <param name="vehicleIndex">Index of the excluded vehicle</param>
        /// <param name="position">It will be instantiated at the closest waypoint from the position sent as parameter</param>
        public static void AddExcludedVehicle(int vehicleIndex, Vector3 position)
        {
            AddExcludedVehicle(vehicleIndex, position, null);
        }


        /// <summary>
        /// This will instantiate an excluded vehicle, the vehicle will work normally, but when it is removed it will not be instantiated again
        /// If the index sent as parameter is not an excluded vehicle, it will be ignored
        /// Call AddExcludedVehicleToSystem to make it behave normally
        /// </summary>
        /// <param name="vehicleIndex">Index of the excluded vehicle</param>
        /// <param name="position">It will be instantiated at the closest waypoint from the position sent as parameter</param>
        /// <param name="completeMethod">Callback triggered after instantiation. It returns the VehicleComponent and the waypoint index where the vehicle was instantiated.</param>
        public static void AddExcludedVehicle(int vehicleIndex, Vector3 position, UnityAction<VehicleComponent, int> completeMethod)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DensityManager?.AddExcludedVehicle(vehicleIndex, position, completeMethod);
#endif
        }


        /// <summary>
        /// Add a traffic vehicle to the closest waypoint from the given position
        /// This method will wait until that vehicle type is available and the closest waypoint will be free to add a new vehicle on it.
        /// The method will run in background until the new vehicle is added.
        /// </summary>
        /// <param name="position">The position where to add a new vehicle</param>
        /// <param name="vehicleType">The type of vehicle to add</param>
        public static void AddVehicle(Vector3 position, VehicleTypes vehicleType)
        {
            AddVehicle(position, vehicleType, null);
        }


        /// <summary>
        /// Add a traffic vehicle to the closest waypoint from the given position
        /// This method will wait until that vehicle type is available and the closest waypoint will be free to add a new vehicle on it.
        /// The method will run in background until the new vehicle is added.
        /// </summary>
        /// <param name="position">The position where to add a new vehicle</param>
        /// <param name="vehicleType">The type of vehicle to add</param>
        /// <param name="completeMethod">Callback triggered after instantiation. It returns the VehicleComponent and the waypoint index where the vehicle was instantiated.</param>
        public static void AddVehicle(Vector3 position, VehicleTypes vehicleType, UnityAction<VehicleComponent, int> completeMethod)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DensityManager?.AddVehicleAtPosition(position, vehicleType, completeMethod, null);
#endif
        }
        #endregion


        #region TrafficVehicles
        /// <summary>
        /// Turn all vehicle lights on or off
        /// </summary>
        /// <param name="on">If true, lights are on</param>
        public static void UpdateVehicleLights(bool on)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.TrafficVehicles?.UpdateVehicleLights(on);
#endif
        }


        /// <summary>
        /// Control the engine volume from your master volume
        /// </summary>
        /// <param name="volume">Current engine AudioSource volume</param>
        public static void SetEngineVolume(float volume)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.TrafficVehicles?.UpdateMasterVolume(volume);
#endif
        }


        /// <summary>
        /// After the vehicle is disabled, it will not be instantiated anymore by the Traffic System
        /// </summary>
        /// <param name="vehicleIndex">Index of the vehicle to be excluded</param>
        public static void ExcludeVehicleFromSystem(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.TrafficVehicles?.ExcludeVehicleFromSystem(vehicleIndex);
#endif
        }


        /// <summary>
        /// Add a previously excluded vehicle back to the Traffic System
        /// </summary>
        /// <param name="vehicleIndex">Index of the vehicle to be added back to the system</param>
        public static void AddExcludedVehicleToSystem(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.TrafficVehicles?.AddExcludecVehicleToSystem(vehicleIndex);
#endif
        }


        /// <summary>
        /// Gets the index of a vehicle GameObject.
        /// </summary>
        /// <param name="vehicle">The root GameObject of a traffic vehicle.</param>
        /// <returns>The list index of the vehicle (-1 = error)</returns>
        public static int GetVehicleIndex(GameObject vehicle)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (TrafficManager.Instance.TrafficVehicles)
            {
                return TrafficManager.Instance.TrafficVehicles.GetVehicleIndex(vehicle);
            }
#endif
            return -1;
        }


        /// <summary>
        /// Gets the Vehicle Component from the vehicle with the index passed as a parameter.
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle to get the component from.</param>
        /// <returns>the component from the vehicle with the index passed as a parameter.</returns>
        public static VehicleComponent GetVehicleComponent(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (TrafficManager.Instance.TrafficVehicles)
            {
                return TrafficManager.Instance.TrafficVehicles.GetVehicleComponent(vehicleIndex);
            }
#endif
            return null;
        }


        /// <summary>
        /// Returns a list of all excluded vehicles. 
        /// </summary>
        /// <returns>A list of all VehicleComponents that are currently excluded</returns>
        public static List<VehicleComponent> GetExcludedVehicleList()
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.TrafficVehicles?.GetExcludedVehicleList();
#else
            return null;
#endif
        }


        /// <summary>
        /// Converts the excluded vehicle GameObject into its corresponding vehicle index.
        /// </summary>
        /// <param name="vehicle">The root GameObject of an excluded vehicle.</param>
        /// <returns>The vehicle index (-1 = error)</returns>
        public static int GetExcludedVehicleIndex(GameObject vehicle)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (TrafficManager.Instance.TrafficVehicles)
            {
                return TrafficManager.Instance.TrafficVehicles.GetExcludedVehicleIndex(vehicle);
            }
#endif
            return -1;
        }


        /// <summary>
        ///If a vehicle detects a collider and that collider is destroyed by another script, 
        ///the OnTriggerExit method is not automatically triggered.
        ///In such cases, this method needs to be manually invoked to remove the obstacle in front of the traffic vehicle.
        /// </summary>
        /// <param name="collider">The removed collider.</param>
        public static void TriggerColliderRemovedEvent(Collider collider)
        {
#if GLEY_TRAFFIC_SYSTEM
            TriggerColliderRemovedEvent(new Collider[] { collider });
#endif
        }

        public static void TriggerColliderRemovedEvent(Collider[] collider)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.TrafficVehicles?.TriggerColliderRemovedEvent(collider);
#endif
        }


        /// <summary>
        /// Get a list of all vehicles used by the Traffic System.
        /// </summary>
        /// <returns>A list with all vehicle components</returns>
        public static List<VehicleComponent> GetVehicleList()
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.TrafficVehicles?.GetVehicleList();
#else
            return null;
#endif
        }


        /// <summary>
        /// Get the current speed in km/h of the vehicle
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle</param>
        /// <returns></returns>
        public static float GetVehicleSpeed(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (TrafficManager.Instance.TrafficVehicles)
            {
                return TrafficManager.Instance.TrafficVehicles.GetCurrentSpeed(vehicleIndex);
            }
#endif
            return 0;
        }


        /// <summary>
        /// Get the current state of the vehicle
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle</param>
        /// <returns></returns>
        public static DriveActions GetVehicleState(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            if (TrafficManager.Instance.TrafficVehicles)
            {
                return TrafficManager.Instance.TrafficVehicles.GetCurrentAction(vehicleIndex);
            }
#endif
            return DriveActions.Continue;
        }
        #endregion


        #region Waypoint
        /// <summary>
        /// Enable all disabled area waypoints
        /// </summary>
        public static void EnableAllWaypoints()
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.WaypointManager?.EnableAllWaypoints();
#endif
        }


        /// <summary>
        /// A specific predefined path can be assigned to any active vehicle within the Traffic System. 
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <param name="pathWaypoints"></param>
        public static void SetVehiclePath(int vehicleIndex, Queue<int> pathWaypoints)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.WaypointManager?.SetAgentPath(vehicleIndex, pathWaypoints);
#endif
        }


        /// <summary>
        /// Remove a predefined path for a vehicle.
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle to remove the path from.</param>
        public static void RemoveVehiclePath(int vehicleIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.WaypointManager?.RemoveAgentPath(vehicleIndex);
#endif
        }


        /// <summary>
        /// Returns the Waypoint object for a given waypoint index.
        /// </summary>
        /// <param name="waypointIndex">The index of the waypoint.</param>
        /// <returns>The Waypoint object at the index position inside the waypoint list</returns>
        public static Waypoint GetWaypointFromIndex(int waypointIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.WaypointManager?.GetWaypoint<Waypoint>(waypointIndex);
#else
            return null;
#endif
        }

        /// <summary>
        /// Add from code an event on a specific waypoint.
        /// </summary>
        /// <param name="waypointIndex">The index of the waypoint on which to add the event.</param>
        /// <param name="data">The event data is utilized to recognize and respond to the event.</param>
        public static void AddWaypointEvent(int waypointIndex, string data)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.WaypointManager?.AddWaypointEvent(waypointIndex, data);
#endif
        }


        /// <summary>
        /// Remove an event from a waypoint.
        /// </summary>
        /// <param name="waypointIndex">The waypoint to remove the event from.</param>
        public static void RemoveWaypointEvent(int waypointIndex)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.WaypointManager?.RemoveWaypointEvent(waypointIndex);
#endif
        }
        #endregion


        #region Intersection
        /// <summary>
        /// Force a road from a traffic light intersection to change to green
        /// </summary>
        /// <param name="intersectionName">Name of the intersection to change</param>
        /// <param name="roadIndex">The road index to change</param>
        /// <param name="doNotChangeAgain">If true that road will stay green until this param is set back to false</param>
        public static void SetIntersectionRoadToGreen(string intersectionName, int roadIndex, bool doNotChangeAgain = false)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.IntersectionManager?.SetRoadToGreen(intersectionName, roadIndex, doNotChangeAgain);
#endif
        }
        #endregion


        #region DrivingAI
        /// <summary>
        /// Enable/disable hazard lights for a vehicle
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle</param>
        /// <param name="activate">True - means hazard lights are on</param>
        public static void SetHazardLights(int vehicleIndex, bool activate)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.SetHazardLights(vehicleIndex, activate);
#endif
        }


        /// <summary>
        /// Add an external driving action to a traffic vehicle to customize its behavior. 
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle.</param>
        /// <param name="action">The DriveAction action to be added</param>
        /// <param name="force">If true, all the preexisting actions for that vehicle will be cleared and just the current one will be added.</param>
        public static void AddDrivingAction(int vehicleIndex, DriveActions action, bool force)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.AddDriveAction(vehicleIndex, action, force);
#endif
        }


        /// <summary>
        /// Remove a specific drive action from a vehicle.
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle.</param>
        /// <param name="action">Action to remove</param>
        public static void RemoveDrivingAction(int vehicleIndex, DriveActions action)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.RemoveDriveAction(vehicleIndex, action);
#endif
        }


        /// <summary>
        /// Force a traffic vehicle to change the lane in the indicated direction. 
        /// </summary>
        /// <param name="active">If true the ChangeLane action will be added to the vehicle, otherwise it will be removed.</param>
        /// <param name="vehicleIndex">The index of the vehicle.</param>
        /// <param name="side">The road side for changing the lane.</param>
        public static void ChangeLane(bool active, int vehicleIndex, RoadSide side = RoadSide.Any)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.DrivingAI?.ChangeLane(active, vehicleIndex, side);
#endif
        }
        #endregion


        #region PathFinding
        /// <summary>
        /// Calculates a path from the current position of the vehicle to a specified destination.
        /// </summary>
        /// <param name="vehicleIndex">The index of the vehicle.</param>
        /// <param name="position">The destination position.</param>
        public static void SetDestination(int vehicleIndex, Vector3 position)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.PathFindingManager?.SetDestination(vehicleIndex, position);
#endif
        }


        /// <summary>
        /// Returns a waypoint path between a start position and an end position for a specific vehicle type.
        /// </summary>
        /// <param name="startPosition">A Vector3 for the initial position.</param>
        /// <param name="endPosition">A Vector3 for the final position.</param>
        /// <param name="vehicleType">The vehicle type for which this path is intended.</param>
        /// <returns>The waypoint indexes of the path between startPosition and endPosition.</returns>
        public static List<int> GetPath(Vector3 startPosition, Vector3 endPosition, VehicleTypes vehicleType)
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.PathFindingManager?.GetPath(startPosition, endPosition, vehicleType);
#else
            return null;
#endif
        }
        #endregion


        #region Crossings
        /// <summary>
        /// Inform the priority pedestrian crossing that pedestrians started to cross 
        /// </summary>
        /// <param name="crossingName">The name of the street crossing</param>
        /// <param name="stop">Stop the cars</param>
        /// <param name="stopUpdate">Currently not used, no automatic update implemented</param>
        public static void SetPriorityCrossingStopState(string crossingName, bool stop, bool stopUpdate = true)
        {
#if GLEY_TRAFFIC_SYSTEM
            TrafficManager.Instance.SetPriorityCrossingState(crossingName, stop, stopUpdate);
#endif
        }


        /// <summary>
        /// Get the state stop state of a crossing
        /// </summary>
        /// <param name="crossingName">The name of the street crossing</param>
        /// <returns>true -> cars will stop</returns>
        public static bool GetPriorityCrossingStopState(string crossingName)
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.GetPriorityCrossingState(crossingName);
#else
            return false;
#endif
        }


        /// <summary>
        /// Get the color of the vehicle traffic light
        /// </summary>
        /// <param name="crossingName">The name of the crossing to check</param>
        /// <returns>The color of the traffic light</returns>
        public static TrafficLightsColor GetTrafficLightsCrossingState(string crossingName)
        {
#if GLEY_TRAFFIC_SYSTEM
            return TrafficManager.Instance.GetTrafficLightsCrossingState(crossingName);
#else
            return TrafficLightsColor.Red;
#endif
        }
        #endregion
    }
}