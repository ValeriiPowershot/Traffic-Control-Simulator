using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Stores road properties
    /// </summary>
    public class Road : RoadBase
    {
        public List<Lane> lanes;
        public int otherLaneLinkDistance;

        public void SetDefaults(int nrOfLanes, float laneWidth, float waypointDistance, int otherLaneLinkDistance)
        {
            this.nrOfLanes = nrOfLanes;
            this.laneWidth = laneWidth;
            this.waypointDistance = waypointDistance;
            this.otherLaneLinkDistance = otherLaneLinkDistance;
        }

        public void SetRoadProperties(int globalMaxSpeed, int nrOfCars, bool leftSideTraffic)
        {
            draw = true;
            lanes = new List<Lane>();
            for (int i = 0; i < nrOfLanes; i++)
            {
                if (!leftSideTraffic)
                {
                    lanes.Add(new Lane(nrOfCars, i % 2 == 0, globalMaxSpeed));
                }
                else
                {
                    lanes.Add(new Lane(nrOfCars, i % 2 != 0, globalMaxSpeed));
                }
            }
        }

        public override bool VerifyAssignments()
        {
            if (path == null || path.NumPoints < 4)
            {
                Debug.LogWarning($"{name} is corrupted and will be deleted");
                return false;
            }

            if (!justCreated)
            {
                if (lanes == null)
                {
                    lanes = new List<Lane>();
                }
                if (waypointDistance <= 0)
                {
                    waypointDistance = 1;
                }
                for (int j = 0; j < lanes.Count; j++)
                {
                    if (lanes[j].laneEdges.inConnector == null || lanes[j].laneEdges.outConnector == null)
                    {
                        Debug.LogError($"{name} is corrupted. Go to Edit Road Window and press Generate Waypoints.", this);
                        return false;
                    }
                }
            }
            return true;
        }

        public void UpdateLaneNumber(int maxSpeed, int nrOfCars)
        {
            if (lanes.Count > nrOfLanes)
            {
                lanes.RemoveRange(nrOfLanes, lanes.Count - nrOfLanes);
            }
            if (lanes.Count < nrOfLanes)
            {
                for (int i = lanes.Count; i < nrOfLanes; i++)
                {
                    lanes.Add(new Lane(nrOfCars, i % 2 == 0, maxSpeed));
                }
            }
        }

        public void AddLaneConnector(WaypointSettingsBase inConnector, WaypointSettingsBase outConnector, int index)
        {
            if (inConnector != null)
            {
                inConnector.name = inConnector.transform.parent.parent.parent.name + "-" + inConnector.transform.parent.name + Gley.UrbanAssets.Internal.Constants.inWaypointEnding;
            }
            if (outConnector != null)
            {
                outConnector.name = outConnector.transform.parent.parent.parent.name + "-" + outConnector.transform.parent.name + Gley.UrbanAssets.Internal.Constants.outWaypointEnding;
            }
            lanes[index].laneEdges = new LaneConnectors(inConnector, outConnector);
        }

        public void SwitchDirection(int laneNumber)
        {
            AddLaneConnector(lanes[laneNumber].laneEdges.outConnector, lanes[laneNumber].laneEdges.inConnector, laneNumber);
        }

        public List<int> GetAllowedCars(int laneNumber)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < lanes[laneNumber].allowedCars.Length; i++)
            {
                if (lanes[laneNumber].allowedCars[i] == true)
                {
                    result.Add(i);
                }
            }
            return result;
        }


        public int GetNrOfLanes()
        {
            return transform.Find(Gley.UrbanAssets.Internal.Constants.lanesHolderName).childCount;
        }
    }
}
