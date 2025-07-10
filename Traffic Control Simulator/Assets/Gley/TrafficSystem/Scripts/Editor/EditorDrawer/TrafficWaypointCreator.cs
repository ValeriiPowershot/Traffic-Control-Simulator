using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficWaypointCreator : Creator
    {
        internal TrafficWaypointCreator Initialize()
        {
            return this;
        }


        internal Transform CreateWaypoint(Transform parent, Vector3 waypointPosition, string name, List<int> allowedCars, int maxSpeed, float laneWidth)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(parent);
            go.transform.position = waypointPosition;
            go.name = name;
            go.tag = UrbanAssets.Internal.Constants.editorTag;
            WaypointSettings waypointScript = go.AddComponent<WaypointSettings>();
            waypointScript.Initialize();
            waypointScript.allowedCars = allowedCars.Cast<VehicleTypes>().ToList();
            waypointScript.maxSpeed = maxSpeed;
            waypointScript.laneWidth = laneWidth;
            return go.transform;
        }
    }
}
