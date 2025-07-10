using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    public class PathFindingData : MonoBehaviour
    {
        const int changeLanePenalty = 10;

        public List<PathFindingWaypoint> allPathFindingWaypoints;


#if UNITY_EDITOR
        public void GenerateWaypoints(List<WaypointSettings> allEditorWaypoints)
        {
            allPathFindingWaypoints = new List<PathFindingWaypoint>();
            for (int i = 0; i < allEditorWaypoints.Count; i++)
            {
                List<int> penalties = new List<int>();
                List<WaypointSettingsBase> neighbors = new List<WaypointSettingsBase>();

                for (int j = 0; j < allEditorWaypoints[i].neighbors.Count; j++)
                {
                    neighbors.Add(allEditorWaypoints[i].neighbors[j]);
                    penalties.Add(allEditorWaypoints[i].neighbors[j].penalty);
                }
                for (int j = 0; j < allEditorWaypoints[i].otherLanes.Count; j++)
                {
                    neighbors.Add(allEditorWaypoints[i].otherLanes[j]);
                    penalties.Add(allEditorWaypoints[i].otherLanes[j].penalty + changeLanePenalty);
                }

                allPathFindingWaypoints.Add(new PathFindingWaypoint(allEditorWaypoints[i].name, i, allEditorWaypoints[i].transform.position, 0, 0, -1, neighbors.ToListIndex(allEditorWaypoints), penalties, allEditorWaypoints[i].allowedCars));
            }
        }
#endif
    }
}