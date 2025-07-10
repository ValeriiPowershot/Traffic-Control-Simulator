using Gley.UrbanAssets.Editor;
using System.Collections.Generic;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficSettingsWindowData : SettingsWindowData
    {
        public RoutesColors speedRoutes;

        public bool showOtherLanes;
        public bool showSpeed;


        public bool viewRoadLaneChanges;

        public bool leftSideTraffic;

        public int maxSpeed;
        public int nrOfLanes;
        public int otherLaneLinkDistance;

        public List<VehicleTypes> globalCarList = new List<VehicleTypes>();

        public bool showAllIntersections;
        public bool showExitWaypoints = true;
        public bool showPedestrianWaypoints = true;
        public bool showDirectionWaypoints = true;

        internal override SettingsWindowData Initialize()
        {
            if (nrOfLanes == default)
            {
                nrOfLanes = 2;
            }
            if (laneWidth == default)
            {
                laneWidth = 4;
            }
            if (waypointDistance == default)
            {
                waypointDistance = 4;
            }
            if (maxSpeed == default)
            {
                maxSpeed = 50;
            }

            if (otherLaneLinkDistance == default)
            {
                otherLaneLinkDistance = 1;
            }
            return this;
        }
    }
}