using UnityEngine;

namespace Gley.TrafficSystem
{
    [System.Serializable]
    public struct Area
    {
        public Vector3 center;
        public float radius;
        [HideInInspector]
        public float sqrRadius;

        public Area(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
            sqrRadius = radius * radius;
        }
    }


    public class TrafficOptions
    {
        public float minDistanceToAdd = -1;
        public float distanceToRemove = -1;
        public float masterVolume = 1;
        public bool useWaypointPriority = false;
        public float greenLightTime = -1;
        public float yellowLightTime = -1;
        public int activeSquaresLevel = 1;
        public int initialDensity = -1; //all vehicles are available from the start
        public bool lightsOn = false;
        public Area disableWaypointsArea = default;


        private TrafficLightsBehaviour trafficLightsBehaviour;
        public TrafficLightsBehaviour TrafficLightsBehaviour
        {
            get
            {
                if (trafficLightsBehaviour == null)
                {
                    trafficLightsBehaviour = DefaultDelegates.TrafficLightBehaviour;
                }
                return trafficLightsBehaviour;
            }
            set
            {
                trafficLightsBehaviour = value;
            }
        }


        private SpawnWaypointSelector spawnWaypointSelector;
        public SpawnWaypointSelector SpawnWaypointSelector
        {
            get
            {
                if (spawnWaypointSelector == null)
                {
                    spawnWaypointSelector = DefaultDelegates.GetRandomSpawnWaypoint;
                }
                return spawnWaypointSelector;
            }
            set
            {
                spawnWaypointSelector = value;
            }
        }


        private ModifyTriggerSize modifyTriggerSize;
        public ModifyTriggerSize ModifyTriggerSize
        {
            get
            {
                if (modifyTriggerSize == null)
                {
                    modifyTriggerSize = DefaultDelegates.TriggerSizeModifier;
                }
                return modifyTriggerSize;
            }
            set
            {
                modifyTriggerSize = value;
            }
        }


        private PlayerInTrigger playerInTrigger;
        public PlayerInTrigger PlayerInTrigger
        {
            get
            {
                if (playerInTrigger == null)
                {
                    playerInTrigger = DefaultDelegates.PlayerInTriggerBehaviour;
                }
                return playerInTrigger;
            }
            set
            {
                playerInTrigger = value;
            }
        }


        private DynamicObstacleInTrigger dynamicObstacleInTrigger;
        public DynamicObstacleInTrigger DynamicObstacleInTrigger
        {
            get
            {
                if (dynamicObstacleInTrigger == null)
                {
                    dynamicObstacleInTrigger = DefaultDelegates.DynamicObstacleInTriggerBehaviour;
                }
                return dynamicObstacleInTrigger;
            }
            set
            {
                dynamicObstacleInTrigger = value;
            }
        }


        private BuildingInTrigger buildingInTrigger;
        public BuildingInTrigger BuildingInTrigger
        {
            get
            {
                if (buildingInTrigger == null)
                {
                    buildingInTrigger = DefaultDelegates.BuildingInTriggerBehaviour;
                }
                return buildingInTrigger;
            }
            set
            {
                buildingInTrigger = value;
            }
        }


        private VehicleCrash vehicleCrash;
        public VehicleCrash VehicleCrash
        {
            get
            {
                if (vehicleCrash == null)
                {
                    vehicleCrash = DefaultDelegates.VehicleCrashBehaviour;
                }
                return vehicleCrash;
            }
            set
            {
                vehicleCrash = value;
            }
        }
    }
}