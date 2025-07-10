using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class CidySetup : SetupWindowBase
    {
#if GLEY_CIDY_TRAFFIC
        IntersectionType selectedType;
        private float greenLightTime = 10;
        private float yellowLightTime = 3;
        private int waypointSpeed = 50;
        private bool linkLanes = true;
        private int linkDistance = 3;
#endif

        protected override void TopPart()
        {
            base.TopPart();
#if GLEY_CIDY_TRAFFIC
            if (GUILayout.Button("Disable Cidy"))
            {
                Gley.Common.PreprocessorDirective.AddToCurrent(Constants.GLEY_CIDY_TRAFFIC, true);
            }
#else
            if (GUILayout.Button("Enable Cidy Support"))
            {
                Gley.Common.PreprocessorDirective.AddToCurrent(Constants.GLEY_CIDY_TRAFFIC, false);
            }
#endif
            EditorGUILayout.Space();
            if (GUILayout.Button("Download Cidy 2"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/level-design/cidy-2-55298?aid=1011l8QY4");
            }
        }

        protected override void ScrollPart(float width, float height)
        {
            base.ScrollPart(width, height);

#if GLEY_CIDY_TRAFFIC
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select default intersection type to use:");
            selectedType = (IntersectionType)EditorGUILayout.EnumPopup("Intersection type:", selectedType);

            if (selectedType == IntersectionType.TrafficLights)
            {
                greenLightTime = EditorGUILayout.FloatField("Green Light Time", greenLightTime);
                yellowLightTime = EditorGUILayout.FloatField("Yellow Light Time", yellowLightTime);
            }
            EditorGUILayout.Space();

            waypointSpeed = EditorGUILayout.IntField("Max Speed", waypointSpeed);
            EditorGUILayout.Space();

            linkLanes = EditorGUILayout.Toggle("Link lanes for overtake", linkLanes);
            if (linkLanes)
            {
                linkDistance = EditorGUILayout.IntField("Waypoint distance", linkDistance);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Extract Waypoints"))
            {
                List<int> vehicleTypes = System.Enum.GetValues(typeof(VehicleTypes)).Cast<int>().ToList();
                CidyMethods.ExtractWaypoints(selectedType, greenLightTime, yellowLightTime, waypointSpeed, vehicleTypes, linkDistance);
            }
            EditorGUILayout.Space();
#endif

        }
    }
}
