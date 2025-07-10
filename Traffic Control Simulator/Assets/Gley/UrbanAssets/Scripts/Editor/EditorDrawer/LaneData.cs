using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    internal struct LaneHolder<T> where T : WaypointSettingsBase
    {
        public string name;
        public T[] waypoints;
        public bool isClosed;

        public LaneHolder(string name, T[] waypoints, bool isClosed)
        {
            this.name = name;
            this.waypoints = waypoints;
            this.isClosed = isClosed;
        }
    }


    public class LaneData<T, R> : Data where T : RoadBase where R : WaypointSettingsBase
    {
        private RoadData<T> roadData;
        private Dictionary<T, LaneHolder<R>[]> allLanes;

        protected void Initialize(RoadData<T> roadData)
        {
            this.roadData = roadData;
            base.Initialize();
        }


        internal LaneHolder<R>[] GetRoadLanes(T road)
        {
            allLanes.TryGetValue(road, out var lanes);
            return lanes;
        }


        protected override void LoadAllData()
        {
            allLanes = new Dictionary<T, LaneHolder<R>[]>();
            var allRoads = roadData.GetAllRoads();
            for (int i = 0; i < allRoads.Length; i++)
            {
                LaneHolder<R>[] lanes = LoadLanes(allRoads[i]);
                if (lanes != null)
                {
                    allLanes.Add(allRoads[i], lanes);
                }
            }
        }


        private LaneHolder<R>[] LoadLanes(T road)
        {
            Transform lanesHolder = road.transform.Find(UrbanAssets.Internal.Constants.lanesHolderName);
            if (lanesHolder == null || lanesHolder.childCount == 0)
            {
                if (!road.justCreated)
                {
                    Debug.LogWarning($"{road.name} has no lanes. Go to Edit { typeof(T).Name } Window and press Generate Waypoints", road);
                }
                return null;
            }

            var laneList = new List<LaneHolder<R>>();
            for (int i = 0; i < lanesHolder.childCount; i++)
            {
                int nrOfWaypoints = lanesHolder.GetChild(i).childCount;
                if (nrOfWaypoints == 0)
                {
                    Debug.LogWarning($"Lane {lanesHolder.GetChild(i)} from road {road.name} has no Waypoints. Go to Edit {typeof(T).Name} Window and press Generate Waypoints", road);
                    return null;
                }
                Transform lane = lanesHolder.GetChild(i);
                var laneWaypoints = new List<R>();
                for (int j = 0; j < nrOfWaypoints; j++)
                {
                    R waypointScript = lane.GetChild(j).GetComponent<R>();
                    if (waypointScript != null)
                    {
                        waypointScript.VerifyAssignments();
                        waypointScript.position = waypointScript.transform.position;
                        laneWaypoints.Add(waypointScript);
                    }
                }
                if (laneWaypoints[laneWaypoints.Count - 1].neighbors.Contains(laneWaypoints[0]))
                {
                    laneList.Add(new LaneHolder<R>(lane.name, laneWaypoints.ToArray(), true));
                }
                else
                {
                    laneList.Add(new LaneHolder<R>(lane.name, laneWaypoints.ToArray(), false));
                }
            }
            return laneList.ToArray();
        }
    }
}