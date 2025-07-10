using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficRoadCreator : RoadCreator<Road, ConnectionPool, ConnectionCurve>
    {
        internal TrafficRoadCreator Initialize(TrafficRoadData data)
        {
            base.Initialize(data);
            return this;
        }

        internal Road Create(string waypointsHolderName, int nrOfLanes, float laneWidth, float waypointDistance, string prefix, Vector3 firstClick, Vector3 secondClick, int globalMaxSpeed, int nrOfAgents, bool leftSideTraffic, int otherLaneLinkDistance)
        {
            int roadNumber = GetFreeRoadNumber(waypointsHolderName);
            GameObject roadHolder = new GameObject(prefix + "_" + roadNumber);
            roadHolder.tag = UrbanAssets.Internal.Constants.editorTag;
            roadHolder.transform.SetParent(GetRoadWaypointsHolder(waypointsHolderName));
            roadHolder.transform.SetSiblingIndex(roadNumber);
            roadHolder.transform.position = firstClick;
            var road = roadHolder.AddComponent<Road>();
            road.SetDefaults(nrOfLanes, laneWidth, waypointDistance,otherLaneLinkDistance);
            road.CreatePath(firstClick, secondClick);
            road.SetRoadProperties(globalMaxSpeed, nrOfAgents, leftSideTraffic);
            road.justCreated = true;
            EditorUtility.SetDirty(road);
            AssetDatabase.SaveAssets();
            data.TriggerModifiedEvent();
            return road;
        }
    }
}
