using Gley.UrbanAssets.Internal;
using UnityEngine;
namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Store connection curve parameters
    /// </summary>
    [System.Serializable]
    public class ConnectionCurve : ConnectionCurveBase
    {
        [HideInInspector]
        public string name;
        public Transform holder;
        public Path curve;
        public Road fromRoad;
        public Road toRoad;
        public int fromIndex;
        public int toIndex;

        public bool draw;
        public bool drawWaypoints;
        public Vector3 inPosition;
        public Vector3 outPosition;
        public bool inView;


        public ConnectionCurve(Path curve, Road fromRoad, int fromIndex, Road toRoad, int toIndex, bool draw, Transform holder)
        {
            name = holder.name;
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
            this.curve = curve;
            this.fromRoad = fromRoad;
            this.toRoad = toRoad;
            this.draw = draw;
            this.holder = holder;
        }

        public bool VerifyAssignments()
        {
            if (holder == null)
                return false;

            if (fromRoad == null)
                return false;

            if (toRoad == null)
                return false;

            if (fromIndex < 0)
                return false;

            if (toIndex < 0)
                return false;
            return true;
        }

        public WaypointSettings GetOutConnector()
        {
            return (WaypointSettings)fromRoad.lanes[fromIndex].laneEdges.outConnector;
        }

        public WaypointSettings GetInConnector()
        {
            return (WaypointSettings)toRoad.lanes[toIndex].laneEdges.inConnector;
        }

        public string GetName()
        {
            return name;
        }

        public Path GetCurve()
        {
            return curve;
        }

        public Vector3 GetOffset()
        {
            return fromRoad.positionOffset;
        }

        public Transform GetHolder()
        {
            return holder;
        }

        public bool ContainsRoad(Road road)
        {
            if (toRoad == road || fromRoad == road)
            {
                return true;
            }
            return false;
        }

        public bool ContainsLane(Road road, int laneIndex)
        {
            if ((fromRoad == road && fromIndex == laneIndex) || toRoad == road && toIndex == laneIndex)
            {
                return true;
            }
            return false;
        }
    }
}