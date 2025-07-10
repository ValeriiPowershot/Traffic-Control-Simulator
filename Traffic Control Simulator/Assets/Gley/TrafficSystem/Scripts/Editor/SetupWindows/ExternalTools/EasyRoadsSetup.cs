using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Gley.TrafficSystem.Editor
{
    public class EasyRoadsSetup : SetupWindowBase
    {
#if GLEY_EASYROADS_TRAFFIC
        IntersectionType selectedType;
        private float greenLightTime = 10;
        private float yellowLightTime = 3;
        private bool linkLanes = true;
        private int linkDistance = 3;
#endif

        protected override void TopPart()
        {
            base.TopPart();
#if GLEY_EASYROADS_TRAFFIC
            if (GUILayout.Button("Disable Easy Roads"))
            {
                Gley.Common.PreprocessorDirective.AddToCurrent(Constants.GLEY_EASYROADS_TRAFFIC, true);
            }
#else
            if (GUILayout.Button("Enable Easy Roads Support"))
            {
                Gley.Common.PreprocessorDirective.AddToCurrent(Constants.GLEY_EASYROADS_TRAFFIC, false);
            }
#endif

            EditorGUILayout.Space();
            if (GUILayout.Button("Download Easy Roads 3D"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/terrain/easyroads3d-pro-v3-469?aid=1011l8QY4");
            }

        }


        protected override void ScrollPart(float width, float height)
        {
            base.ScrollPart(width, height);

#if GLEY_EASYROADS_TRAFFIC
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select default intersection type to use:");
            selectedType = (IntersectionType)EditorGUILayout.EnumPopup("Intersection type:", selectedType);

            if (selectedType == IntersectionType.TrafficLights)
            {
                greenLightTime = EditorGUILayout.FloatField("Green Light Time", greenLightTime);
                yellowLightTime = EditorGUILayout.FloatField("Yellow Light Time", yellowLightTime);
            }

            linkLanes = EditorGUILayout.Toggle("Link lanes for overtake", linkLanes);
            if (linkLanes)
            {
                linkDistance = EditorGUILayout.IntField("Waypoint distance", linkDistance);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Extract Waypoints"))
            {
                List<int> vehicleTypes = System.Enum.GetValues(typeof(VehicleTypes)).Cast<int>().ToList();
                EasyRoadsMethods.ExtractWaypoints(selectedType, greenLightTime, yellowLightTime, linkLanes, linkDistance, vehicleTypes);
            }
            EditorGUILayout.Space();
#endif
        }

        protected override void BottomPart()
        {
            if (GUILayout.Button("Tutorial Part 1 Basic"))
            {
                Application.OpenURL("https://youtu.be/-GWru2d7fMs");
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Tutorial Part 2 Fix Errors"))
            {
                Application.OpenURL("https://youtu.be/vTAW0jilybI");
            }
            //base.BottomPart();
        }
    }
}
