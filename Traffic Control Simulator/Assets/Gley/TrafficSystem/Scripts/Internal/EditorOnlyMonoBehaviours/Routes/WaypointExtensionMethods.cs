#if UNITY_EDITOR
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Converts waypoints from editor version to runtime version
    /// </summary>
    public static class WaypointExtensionMethods
    {
        public static List<int> ToListIndex(this List<WaypointSettings> editorWaypoints, List<WaypointSettings> allWaypoints)
        {
            List<int> result = new List<int>();
            if (editorWaypoints != null)
            {
                for (int i = 0; i < editorWaypoints.Count; i++)
                {
                    int index = allWaypoints.IndexOf(editorWaypoints[i]);
                    if (index != -1)
                    {
                        result.Add(index);
                    }
                }
            }
            return result;
        }


        public static List<int> ToListIndex(this List<WaypointSettingsBase> editorWaypoints, List<WaypointSettings> allWaypoints)
        {
            List<int> result = new List<int>();
            if (editorWaypoints != null)
            {
                for (int i = 0; i < editorWaypoints.Count; i++)
                {
                    if (editorWaypoints[i] != null)
                    {
                        int index = allWaypoints.IndexOf((WaypointSettings)editorWaypoints[i]);
                        if (index != -1)
                        {
                            result.Add(index);
                        }
                    }
                }
            }
            return result;
        }


        public static int ToListIndex(this WaypointSettings editorWaypoint, List<WaypointSettings> allWaypoints)
        {
            return allWaypoints.IndexOf(editorWaypoint);
        }


        public static List<Waypoint> ToPlayWaypoints(this List<WaypointSettings> editorWaypoints, List<WaypointSettings> allWaypoints)
        {
            List<Waypoint> result = new List<Waypoint>();
            if (editorWaypoints != null)
            {
                for (int i = 0; i < editorWaypoints.Count; i++)
                {
                    result.Add(editorWaypoints[i].ToPlayWaypoint(allWaypoints));
                }
            }
            return result;
        }

        public static Waypoint ToPlayWaypoint(this WaypointSettings editorWaypoint, List<WaypointSettings> allWaypoints)
        {
            return new Waypoint(editorWaypoint.name,
                editorWaypoint.ToListIndex(allWaypoints),
                editorWaypoint.transform.position,
                editorWaypoint.allowedCars,
                editorWaypoint.neighbors.ToListIndex(allWaypoints),
                editorWaypoint.prev.ToListIndex(allWaypoints),
                editorWaypoint.otherLanes.ToListIndex(allWaypoints),
                editorWaypoint.maxSpeed,
                editorWaypoint.priority,
                editorWaypoint.giveWay,
                editorWaypoint.complexGiveWay,
                editorWaypoint.zipperGiveWay,
                editorWaypoint.triggerEvent,
                editorWaypoint.laneWidth,
                editorWaypoint.eventData,
                editorWaypoint.giveWayList.ToListIndex(allWaypoints));
        }


        public static WaypointSettings ToEditorWaypoint(this Waypoint playWaypoint)
        {
            GameObject[] objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == playWaypoint.name && obj.transform.position == playWaypoint.position).ToArray();
            if (objects.Length != 0)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    Debug.Log(objects[i], objects[i]);
                }
            }
            return objects[0].GetComponent<WaypointSettings>();
        }


        public static List<WaypointSettings> ToEditorWaypoints(this List<Waypoint> playWaypoints)
        {
            List<WaypointSettings> result = new List<WaypointSettings>();
            for (int i = 0; i < playWaypoints.Count; i++)
            {
                result.Add(playWaypoints[i].ToEditorWaypoint());
            }
            return result;
        }


        public static List<IntersectionStopWaypoints> ToPlayWaypoints(this List<IntersectionStopWaypointsSettings> giveWayWaypoints, List<WaypointSettings> allWaypoints)
        {
            List<IntersectionStopWaypoints> result = new List<IntersectionStopWaypoints>();
            for (int i = 0; i < giveWayWaypoints.Count; i++)
            {
                result.Add(new IntersectionStopWaypoints(giveWayWaypoints[i].roadWaypoints.ToPlayWaypoints(allWaypoints)));
            }
            return result;
        }


        public static List<IntersectionStopWaypointsIndex> ToPlayIndex(this List<IntersectionStopWaypointsSettings> giveWayWaypoints, List<WaypointSettings> allWaypoints)
        {
            List<IntersectionStopWaypointsIndex> result = new List<IntersectionStopWaypointsIndex>();
            for (int i = 0; i < giveWayWaypoints.Count; i++)
            {
                result.Add(new IntersectionStopWaypointsIndex(giveWayWaypoints[i].roadWaypoints.ToListIndex(allWaypoints), giveWayWaypoints[i].redLightObjects, giveWayWaypoints[i].yellowLightObjects, giveWayWaypoints[i].greenLightObjects, giveWayWaypoints[i].greenLightTime ));
            }
            return result;
        }
    }
}
#endif
