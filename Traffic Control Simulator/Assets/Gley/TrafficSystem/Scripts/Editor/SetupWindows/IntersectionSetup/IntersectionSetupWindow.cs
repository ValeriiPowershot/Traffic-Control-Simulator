using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class IntersectionSetupWindow : TrafficSetupWindow
    {
        PriorityIntersectionSettings[] allPriorityIntersections;
        TrafficLightsIntersectionSettings[] allTrafficLightsIntersections;
        TrafficLightsCrossingSettings[] allTrafficLightsCrossings;
        PriorityCrossingSettings[] allPriorityCrossings;

        private IntersectionData intersectionData;
        private IntersectionDrawer intersectionsDrawer;
        private IntersectionCreator intersectionCreator;
        private readonly float scrollAdjustment = 246;

        private int nrOfPriorityIntersections;
        private int nrOfTrafficLightsIntersections;
        private int nrOfTrafficLightsCrossings;
        private int nrOfPriorityCrossings;
        bool refresh;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            intersectionData = CreateInstance<IntersectionData>().Initialize();
            intersectionsDrawer = CreateInstance<IntersectionDrawer>().Initialize(intersectionData);
            intersectionCreator = CreateInstance<IntersectionCreator>().Initialize(intersectionData);
            intersectionsDrawer.onIntersectionClicked += IntersectionClicked;
            return this;
        }


        public override void DrawInScene()
        {
            refresh = false;
            allPriorityIntersections = intersectionsDrawer.DrawPriorityIntersections(true, editorSave.editorColors.intersectionColor, editorSave.editorColors.stopWaypointsColor, editorSave.editorColors.exitWaypointsColor, editorSave.editorColors.labelColor);
            allPriorityCrossings = intersectionsDrawer.DrawPriorityCrossings(true, editorSave.editorColors.intersectionColor, editorSave.editorColors.stopWaypointsColor);
            allTrafficLightsIntersections = intersectionsDrawer.DrawTrafficLightsIntersections(true, editorSave.editorColors.intersectionColor, editorSave.editorColors.stopWaypointsColor, editorSave.editorColors.exitWaypointsColor, editorSave.editorColors.labelColor);
            allTrafficLightsCrossings = intersectionsDrawer.DrawTrafficLightsCrossings(true, editorSave.editorColors.intersectionColor, editorSave.editorColors.stopWaypointsColor);

            if (nrOfPriorityIntersections != allPriorityIntersections.Length)
            {
                nrOfPriorityIntersections = allPriorityIntersections.Length;
                refresh = true;
            }

            if (nrOfPriorityCrossings != allPriorityCrossings.Length)
            {
                nrOfPriorityCrossings = allPriorityCrossings.Length;
                refresh = true;
            }

            if (nrOfTrafficLightsIntersections != allTrafficLightsIntersections.Length)
            {
                nrOfTrafficLightsIntersections = allTrafficLightsIntersections.Length;
                refresh = true;
            }

            if (nrOfTrafficLightsCrossings != allTrafficLightsCrossings.Length)
            {
                nrOfTrafficLightsCrossings = allTrafficLightsCrossings.Length;
                refresh = true;
            }

            if (refresh)
            {
                SettingsWindowBase.TriggerRefreshWindowEvent();
            }

            base.DrawInScene();
        }


        protected override void TopPart()
        {
            base.TopPart();
            if (GUILayout.Button("Create Priority Intersection"))
            {
                IntersectionClicked(intersectionCreator.Create<PriorityIntersectionSettings>());
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Priority Crossing"))
            {
                IntersectionClicked(intersectionCreator.Create<PriorityCrossingSettings>());
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Traffic Lights Intersection"))
            {
                IntersectionClicked(intersectionCreator.Create<TrafficLightsIntersectionSettings>());
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Traffic Lights Crossing"))
            {
                IntersectionClicked(intersectionCreator.Create<TrafficLightsCrossingSettings>());
            }
            EditorGUILayout.Space();

            editorSave.showAllIntersections = EditorGUILayout.Toggle("Show All Intersections", editorSave.showAllIntersections);
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            if (editorSave.showAllIntersections)
            {
                allPriorityCrossings = intersectionData.GetPriorityCrossings();
                allPriorityIntersections = intersectionData.GetPriorityIntersections();
                allTrafficLightsCrossings = intersectionData.GetTrafficLightsCrossings();
                allTrafficLightsIntersections = intersectionData.GetTrafficLightsIntersections();
            }

            if (allPriorityIntersections != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Priority Intersections");
                for (int i = 0; i < allPriorityIntersections.Length; i++)
                {
                    DrawIntersectionButton(allPriorityIntersections[i]);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (allPriorityCrossings != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Priority Crossings");
                for (int i = 0; i < allPriorityCrossings.Length; i++)
                {
                    DrawIntersectionButton(allPriorityCrossings[i]);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (allTrafficLightsIntersections != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Traffic Light Intersections");
                for (int i = 0; i < allTrafficLightsIntersections.Length; i++)
                {
                    DrawIntersectionButton(allTrafficLightsIntersections[i]);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (allTrafficLightsCrossings != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Traffic Light Crossings");
                for (int i = 0; i < allTrafficLightsCrossings.Length; i++)
                {
                    DrawIntersectionButton(allTrafficLightsCrossings[i]);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            GUILayout.EndScrollView();
        }


        private void IntersectionClicked(GenericIntersectionSettings clickedIntersection)
        {
            SettingsWindow.SetSelectedIntersection(clickedIntersection);
            if (clickedIntersection.GetType().Equals(typeof(TrafficLightsIntersectionSettings)))
            {
                window.SetActiveWindow(typeof(TrafficLightsIntersectionWindow), true);
            }
            if (clickedIntersection.GetType().Equals(typeof(PriorityIntersectionSettings)))
            {
                window.SetActiveWindow(typeof(PriorityIntersectionWindow), true);
            }
            if (clickedIntersection.GetType().Equals(typeof(TrafficLightsCrossingSettings)))
            {
                window.SetActiveWindow(typeof(TrafficLightsCrossingWindow), true);
            }
            if (clickedIntersection.GetType().Equals(typeof(PriorityCrossingSettings)))
            {
                window.SetActiveWindow(typeof(PriorityCrossingWindow), true);
            }
        }


        private void DrawIntersectionButton(GenericIntersectionSettings intersection)
        {
            if(intersection==null)
            {
                return; 
            }
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(intersection.name);
            if (GUILayout.Button("View", GUILayout.Width(BUTTON_DIMENSION)))
            {
                GleyUtilities.TeleportSceneCamera(intersection.transform.position, 10);
            }
            if (GUILayout.Button("Edit", GUILayout.Width(BUTTON_DIMENSION)))
            {
                IntersectionClicked(intersection);
            }
            if (GUILayout.Button("Delete", GUILayout.Width(BUTTON_DIMENSION)))
            {
                intersectionCreator.DeleteIntersection(intersection);
            }
            EditorGUILayout.EndHorizontal();
        }

        

        public override void DestroyWindow()
        {
            intersectionsDrawer.onIntersectionClicked -= IntersectionClicked;

            DestroyImmediate(intersectionsDrawer);
            DestroyImmediate(intersectionData);
            DestroyImmediate(intersectionCreator);

            base.DestroyWindow();
        }
    }
}
