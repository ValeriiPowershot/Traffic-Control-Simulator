using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TrafficControl.CleanVehicles;

namespace TrafficControl.EditorTools
{
    public static class CityLevelPreviewTools
    {
        private const string Source = "Assets/_Project/Scenes/Production/Game/Level1.unity";
        private const string Target = "Assets/_Project/Sandbox/Scenes/ControllerTests/Level1CleanTraffic.unity";
        [MenuItem("Traffic Control/Experimental/Open Level 1 Clean Traffic Preview")]
        public static void Open()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!AssetDatabase.LoadAssetAtPath<SceneAsset>(Target)) { Create(); return; }
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(Target);
        }
        [MenuItem("Traffic Control/Experimental/Create Level 1 Clean Traffic Preview")]
        public static void Create()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.OpenScene(Source);
            // Save the copy before changing anything in the scene.
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Target));
            AssetDatabase.Refresh();
            string target = AssetDatabase.GenerateUniqueAssetPath(Target);
            if (string.IsNullOrEmpty(target)) { Debug.LogError("Cannot create preview scene path."); return; }
            if (!EditorSceneManager.SaveScene(scene, target)) return;
            var sources = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<BaseCode.CarSpawnerSystem>(true)).ToArray();
            var signals = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<TrafficLight>(true)).Where(light => light.isActiveAndEnabled).ToArray();
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var bootstrap in root.GetComponentsInChildren<CampaignBootstrap>(true)) bootstrap.gameObject.SetActive(false);
                foreach (var manager in root.GetComponentsInChildren<BaseCode.Logic.City.GameManager>(true)) manager.gameObject.SetActive(false);
                foreach (var car in root.GetComponentsInChildren<Realistic_Traffic_Controller.Scripts.RTC_CarController>(true)) car.gameObject.SetActive(false);
                foreach (var canvas in root.GetComponentsInChildren<Canvas>(true)) canvas.enabled = false;
                foreach (var difficulty in root.GetComponentsInChildren<DifficultyController>(true)) difficulty.enabled = false;
            }
            var container = new GameObject("Clean traffic preview");
            var spawns = new List<CityCarSpawner>();
            Physics.SyncTransforms();
            foreach (var source in sources)
            {
                source.enabled = false;
                if (source.spawners == null) continue;
                foreach (var config in source.spawners)
                {
                    if (config == null || !config.StartWaypoint) continue;
                    var points = new List<Vector3>(); var visited = new HashSet<RTC_Waypoint>();
                    var waypoint = config.StartWaypoint;
                    while (waypoint && visited.Add(waypoint) && points.Count < 256)
                    {
                        Vector3 p = waypoint.transform.position;
                        // Use the actual road surface, not a waypoint's decorative height.
                        var hits = Physics.RaycastAll(p + Vector3.up * 2, Vector3.down, 6, ~0, QueryTriggerInteraction.Ignore);
                        var ground = hits.Where(h => h.normal.y > 0.85f && !h.collider.GetComponentInParent<CityCarController>())
                            .OrderBy(h => Mathf.Abs(h.point.y - p.y)).ToArray();
                        if (ground.Length > 0) p.y = ground[0].point.y;
                        points.Add(p); waypoint = waypoint.nextWaypoint ? waypoint.nextWaypoint : waypoint.interConnectionWaypoint;
                    }
                    if (points.Count < 2) { Debug.LogWarning("No complete route for stream " + config.SpawnPointIndex); continue; }
                    var go = new GameObject("Stream " + config.SpawnPointIndex); go.transform.SetParent(container.transform);
                    var road = go.AddComponent<CityRoadPath>(); road.Curves = CityRouteBuilder.Build(points); road.Rebuild();
                    var spawn = go.AddComponent<CityCarSpawner>(); spawn.Road = road; spawn.StartAutomatically = false;
                    spawn.Interval = Mathf.Max(2, config.SpawnDelay); spawn.Paint = Color.HSVToRGB(spawns.Count * 0.24f, 0.7f, 0.9f); spawns.Add(spawn);
                    foreach (var signal in signals)
                    {
                        if (signal.TrafficLightSpawnIndex != config.SpawnPointIndex || !signal.StopBarrier) continue;
                        var box = signal.StopBarrier as BoxCollider;
                        Vector3 stopCenter = box ? box.transform.TransformPoint(box.center) : signal.StopBarrier.transform.position;
                        float d = road.Project(stopCenter, 0, true);
                        var line = go.AddComponent<CityStopLine>(); line.Road = road; line.Distance = d;
                        var bridge = go.AddComponent<CityTrafficLightBridge>(); bridge.Source = signal; bridge.StopLine = line;
                    }
                }
            }
            var preview = container.AddComponent<CityTrafficPreview>(); preview.Spawners = spawns.ToArray(); preview.Signals = signals;
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = container;
            Debug.Log("Saved independent Level 1 traffic preview: " + target + "; streams=" + spawns.Count);
        }
    }
}
