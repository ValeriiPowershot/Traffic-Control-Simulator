using Gley.UrbanAssets.Internal;
using System;
using System.Collections.Generic;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Stores waypoint properties
    /// </summary>
    public class WaypointSettings : WaypointSettingsBase
    {
        public List<WaypointSettings> otherLanes;
        public List<WaypointSettings> giveWayList;
        public List<VehicleTypes> allowedCars;
        public float laneWidth;
        public int maxSpeed;

        public bool giveWay;
        public bool enter;
        public bool exit;
        public bool speedLocked;
        public bool carsLocked;
        public bool complexGiveWay;
        public bool zipperGiveWay;

        public override void Initialize()
        {
            base.Initialize();
            otherLanes = new List<WaypointSettings>();
            giveWayList = new List<WaypointSettings>();
        }

        public override void VerifyAssignments()
        {
            base.VerifyAssignments();

            if (otherLanes == null)
            {
                otherLanes = new List<WaypointSettings>();
            }

            for (int j = otherLanes.Count - 1; j >= 0; j--)
            {
                if (otherLanes[j] == null)
                {
                    otherLanes.RemoveAt(j);
                }
            }

            if (giveWayList == null)
            {
                giveWayList = new List<WaypointSettings>();
            }

            for (int j = giveWayList.Count - 1; j >= 0; j--)
            {
                if (giveWayList[j] == null)
                {
                    giveWayList.RemoveAt(j);
                }
            }

            if (allowedCars == null)
            {
                allowedCars = new List<VehicleTypes>();
            }

            for (int i = allowedCars.Count - 1; i >= 0; i--)
            {
                if (!IsValid((int)allowedCars[i]))
                {
                    allowedCars.RemoveAt(i);
                }
            }

            if (laneWidth == 0)
            {
                if (name.Contains(Gley.UrbanAssets.Internal.Constants.connect))
                {
                    laneWidth = 4;
                }
                else
                {
                    laneWidth = transform.parent.parent.parent.GetComponent<Road>().laneWidth;
                }
            }
        }
        private bool IsValid(int value)
        {
            return Enum.IsDefined(typeof(VehicleTypes), value);
        }
    }
}