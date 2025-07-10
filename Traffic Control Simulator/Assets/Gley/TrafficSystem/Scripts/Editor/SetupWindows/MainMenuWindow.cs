using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class MainMenuWindow : SetupWindowBase
    {
        private const int scrollAdjustment = 119;
        private const string SAVING = "GleyTrafficSaving";
        private const string STEP = "GleyTrafficStep";

        private int step;
        private bool saving;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            saving = EditorPrefs.GetBool(SAVING);
            step = EditorPrefs.GetInt(STEP);
            return base.Initialize(windowProperties, window);
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            EditorGUILayout.Space();

            if (GUILayout.Button("Import Required Packages"))
            {
                window.SetActiveWindow(typeof(ImportPackagesWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Scene Setup"))
            {
                window.SetActiveWindow(typeof(SceneSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Road Setup"))
            {
                window.SetActiveWindow(typeof(RoadSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Intersection Setup"))
            {
                window.SetActiveWindow(typeof(IntersectionSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Waypoint Setup"))
            {
                window.SetActiveWindow(typeof(WaypointSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Speed Routes Setup"))
            {
                window.SetActiveWindow(typeof(SpeedRoutesSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Vehicle Routes Setup"))
            {
                window.SetActiveWindow(typeof(VehicleRoutesSetupWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Waypoint Priority Setup"))
            {
                window.SetActiveWindow(typeof(WaypointPriorityWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Path Finding"))
            {
                window.SetActiveWindow(typeof(PathFindingWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("External Tools"))
            {
                window.SetActiveWindow(typeof(ExternalToolsWindow), true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Switch driving direction (Beta)"))
            {
                SwitchWaypointDirection.SwitchAll();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Debug"))
            {
                window.SetActiveWindow(typeof(DebugWindow), true);
            }
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
            base.ScrollPart(width, height);
        }


        protected override void BottomPart()
        {
            if (GUILayout.Button("Apply Settings"))
            {
                if (LayerOperations.LoadOrCreateLayers<LayerSetup>(Internal.Constants.layerPath).edited == false)
                {
                    Debug.LogWarning("Layers are not configured. Go to Tools -> Gley -> Traffic System->Scene Setup -> Layer Setup");
                }

                if (FindFirstObjectByType<VehicleComponent>() != null)
                {
                    Debug.LogError("Failed: Please remove VehicleComponent from the following objects:");
                    var objects = FindObjectsByType<VehicleComponent>(FindObjectsSortMode.None);
                    foreach (var obj in objects)
                    {
                        Debug.Log(obj.name, obj);
                    }
                    return;
                }

                step = 0;
                SaveSettings();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://gley.gitbook.io/mobile-traffic-system/quick-start");
            }

            base.BottomPart();
        }


        void SaveSettings()
        {
            Debug.Log($"Saving {step + 1}/4");
            switch (step)
            {
                case 0:
                    if (!File.Exists($"{Application.dataPath}{Internal.Constants.agentTypesPath}/VehicleTypes.cs"))
                    {
                        FileCreator.CreateAgentTypesFile<VehicleTypes>(null, Internal.Constants.GLEY_TRAFFIC_SYSTEM, Internal.Constants.trafficNamespace, Internal.Constants.agentTypesPath);
                    }
                    saving = true;
                    EditorPrefs.SetBool(SAVING, saving);
                    step++;
                    EditorPrefs.SetInt(STEP, step);
                    break;
                case 1:
                    Common.PreprocessorDirective.AddToCurrent(Internal.Constants.GLEY_TRAFFIC_SYSTEM, false);
                    saving = true;
                    EditorPrefs.SetBool(SAVING, saving);
                    step++;
                    EditorPrefs.SetInt(STEP, step);
                    break;
                case 2:
                    GridEditor.ApplySettings(CurrentSceneData.GetSceneInstance());
                    saving = true;
                    EditorPrefs.SetBool(SAVING, saving);
                    step++;
                    EditorPrefs.SetInt(STEP, step);
                    break;
                default:
                    Debug.Log("Save Done");
                    break;
            }
        }


        public override void InspectorUpdate()
        {
            if (saving)
            {
                if (EditorApplication.isCompiling == false)
                {
                    saving = false;
                    EditorPrefs.SetBool(SAVING, false);
                    SaveSettings();
                }
            }
        }
    }
}
