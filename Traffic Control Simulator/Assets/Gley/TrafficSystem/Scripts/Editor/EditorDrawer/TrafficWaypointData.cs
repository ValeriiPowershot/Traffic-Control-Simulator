using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficWaypointData : Data
    {
        private WaypointSettings[] allWaypoints;
        private WaypointSettings[] disconnectedWaypoints;
        private WaypointSettings[] vehicleEditedWaypoints;
        private WaypointSettings[] speedEditedWaypoints;
        private WaypointSettings[] priorityEditedWaypoints;
        private WaypointSettings[] giveWayWaypoints;
        private WaypointSettings[] complexGiveWayWaypoints;
        private WaypointSettings[] zipperGiveWayWaypoints;
        private WaypointSettings[] eventWaypoints;
        private WaypointSettings[] penaltyEditedWaypoints;


        internal new TrafficWaypointData Initialize()
        {
            base.Initialize();
            return this;
        }


        internal WaypointSettings[] GetAllWaypoints()
        {
            return allWaypoints;
        }


        internal WaypointSettings[] GetDisconnectedWaypoints()
        {
            return disconnectedWaypoints;
        }


        internal WaypointSettings[] GetVehicleEditedWaypoints()
        {
            return vehicleEditedWaypoints;
        }


        internal WaypointSettings[] GetSpeedEditedWaypoints()
        {
            return speedEditedWaypoints;
        }


        internal WaypointSettings[] GetPriorityEditedWaypoints()
        {
            return priorityEditedWaypoints;
        }


        internal WaypointSettings[] GetGiveWayWaypoints()
        {
            return giveWayWaypoints;
        }


        internal WaypointSettings[] GetComplexGiveWayWaypoints()
        {
            return complexGiveWayWaypoints;
        }


        internal WaypointSettings[] GetZipperGiveWayWaypoints()
        {
            return zipperGiveWayWaypoints;
        }


        internal WaypointSettings[] GetEventWaypoints()
        {
            return eventWaypoints;
        }


        internal WaypointSettings[] GetPenlatyEditedWaypoints()
        {
            return penaltyEditedWaypoints;
        }


        protected override void LoadAllData()
        {
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                allWaypoints = FindObjectsByType<WaypointSettings>(FindObjectsSortMode.None);
            }
            else
            {
                allWaypoints = GleyPrefabUtilities.GetScenePrefabRoot().GetComponentsInChildren<WaypointSettings>();
            }

            List<WaypointSettings> disconnectedWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> vehicleEditedWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> speedEditedWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> priorityEditedWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> giveWayWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> complexGiveWayWaypoints = new List<WaypointSettings> ();
            List<WaypointSettings> zipperGiveWayWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> eventWaypoints = new List<WaypointSettings>();
            List<WaypointSettings> penaltyEditedWaypoints = new List<WaypointSettings>();

            //initialization and checks
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                allWaypoints[i].position = allWaypoints[i].transform.position;
                allWaypoints[i].VerifyAssignments();


                if (allWaypoints[i].neighbors.Count == 0)
                {
                    disconnectedWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].carsLocked)
                {
                    vehicleEditedWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].speedLocked)
                {
                    speedEditedWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].priorityLocked)
                {
                    priorityEditedWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].giveWay)
                {
                    giveWayWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].complexGiveWay)
                {
                    complexGiveWayWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].zipperGiveWay)
                {
                    zipperGiveWayWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].triggerEvent)
                {
                    eventWaypoints.Add(allWaypoints[i]);
                }

                if (allWaypoints[i].penaltyLocked)
                {
                    penaltyEditedWaypoints.Add(allWaypoints[i]);
                }
            }
            this.disconnectedWaypoints = disconnectedWaypoints.ToArray();
            this.vehicleEditedWaypoints = vehicleEditedWaypoints.ToArray();
            this.speedEditedWaypoints = speedEditedWaypoints.ToArray();
            this.priorityEditedWaypoints = priorityEditedWaypoints.ToArray();
            this.giveWayWaypoints = giveWayWaypoints.ToArray();
            this.complexGiveWayWaypoints = complexGiveWayWaypoints.ToArray();
            this.zipperGiveWayWaypoints = zipperGiveWayWaypoints.ToArray();
            this.eventWaypoints = eventWaypoints.ToArray();
            this.penaltyEditedWaypoints = penaltyEditedWaypoints.ToArray();
        }
    }
}
