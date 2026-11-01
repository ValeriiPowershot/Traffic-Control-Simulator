using System.Collections.Generic;
using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    public sealed class CleanTrafficLab : MonoBehaviour
    {
        private readonly List<CityCarSpawner> spawners = new List<CityCarSpawner>();
        private readonly List<TrafficLight> lights = new List<TrafficLight>();
        private readonly List<MeshRenderer> lamps = new List<MeshRenderer>();
        private readonly string[] labels = { "Юг", "Запад", "Север", "Восток" };
        private bool running;
        private Material asphalt, grass, white, pole;
        private MaterialPropertyBlock lampColor;
        private void Awake()
        {
            lampColor = new MaterialPropertyBlock();
            Time.timeScale = 1;
            asphalt = CityCarFactory.Material(new Color(0.14f, 0.16f, 0.18f));
            grass = CityCarFactory.Material(new Color(0.22f, 0.35f, 0.25f));
            white = CityCarFactory.Material(new Color(0.9f, 0.9f, 0.8f));
            pole = CityCarFactory.Material(new Color(0.06f, 0.07f, 0.08f));
            CityCarFactory.Box("Ground", transform, new Vector3(0, -0.5f, 0), new Vector3(100, 1, 100), grass, true);
            CityCarFactory.Box("North South road", transform, new Vector3(0, 0.01f, 0), new Vector3(9, 0.02f, 75), asphalt);
            CityCarFactory.Box("East West road", transform, new Vector3(0, 0.015f, 0), new Vector3(75, 0.02f, 9), asphalt);
            for (int i = -34; i <= 34; i += 6)
            {
                if (Mathf.Abs(i) < 9) continue;
                CityCarFactory.Box("Lane mark", transform, new Vector3(0, 0.035f, i), new Vector3(0.12f, 0.015f, 2), white);
                CityCarFactory.Box("Lane mark", transform, new Vector3(i, 0.04f, 0), new Vector3(2, 0.015f, 0.12f), white);
            }
            var south = Route("South right turn", new[] {
                CityRoadPath.Curve.Straight(new Vector3(2,0,-34), new Vector3(2,0,-8)),
                new CityRoadPath.Curve { Start=new Vector3(2,0,-8), HandleA=new Vector3(2,0,-4.6863f), HandleB=new Vector3(4.6863f,0,-2), End=new Vector3(8,0,-2), SpeedKph=20 },
                CityRoadPath.Curve.Straight(new Vector3(8,0,-2),new Vector3(34,0,-2)) });
            var west = Route("West straight", new[] { CityRoadPath.Curve.Straight(new Vector3(-34,0,-2),new Vector3(34,0,-2)) });
            var north = Route("North straight", new[] { CityRoadPath.Curve.Straight(new Vector3(-2,0,34),new Vector3(-2,0,-34)) });
            var east = Route("East right turn", new[] {
                CityRoadPath.Curve.Straight(new Vector3(34,0,2),new Vector3(8,0,2)),
                new CityRoadPath.Curve { Start=new Vector3(8,0,2), HandleA=new Vector3(4.6863f,0,2), HandleB=new Vector3(2,0,4.6863f), End=new Vector3(2,0,8), SpeedKph=20 },
                CityRoadPath.Curve.Straight(new Vector3(2,0,8),new Vector3(2,0,34)) });
            AddFlow(south, new Color(0.1f,0.65f,0.9f), new Vector3(5,1.2f,-12));
            AddFlow(west, new Color(0.95f,0.5f,0.15f), new Vector3(-12,1.2f,-5));
            AddFlow(north, new Color(0.65f,0.35f,0.85f), new Vector3(-5,1.2f,12));
            AddFlow(east, new Color(0.9f,0.8f,0.2f), new Vector3(12,1.2f,5));
            var camera = new GameObject("Lab Camera").AddComponent<Camera>(); camera.tag = "MainCamera";
            camera.transform.position = new Vector3(32, 48, -38); camera.transform.LookAt(Vector3.zero);
            camera.orthographic = true; camera.orthographicSize = 32;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(0.16f,0.21f,0.2f);
            var sun = new GameObject("Lab Sun").AddComponent<Light>(); sun.type = LightType.Directional; sun.intensity = 1.3f;
            sun.transform.rotation = Quaternion.Euler(50,-30,0);
            RenderSettings.ambientLight = new Color(0.6f,0.6f,0.6f);
        }
        private CityRoadPath Route(string name, CityRoadPath.Curve[] curves)
        {
            var go = new GameObject(name); go.transform.SetParent(transform, false);
            var road = go.AddComponent<CityRoadPath>(); road.Curves = curves; road.Rebuild(); return road;
        }
        private void AddFlow(CityRoadPath road, Color color, Vector3 signalPosition)
        {
            var go = new GameObject(labels[spawners.Count] + " spawner"); go.transform.SetParent(transform, false);
            var spawn = go.AddComponent<CityCarSpawner>(); spawn.Road = road; spawn.Paint = color; spawn.enabled = false; spawners.Add(spawn);
            var signal = CityCarFactory.Box("Signal " + labels[lights.Count], transform, signalPosition, new Vector3(0.8f,2.4f,0.8f), pole, true);
            var indicator = CityCarFactory.Box("Lamp", signal.transform, new Vector3(0,0.25f,-0.55f), new Vector3(0.65f,0.3f,0.15f), white);
            lamps.Add(indicator.GetComponent<MeshRenderer>());
            var source = signal.AddComponent<TrafficLight>();
            var stop = new GameObject("Stop line state"); stop.transform.SetParent(transform, false);
            var lineSample = road.At(22); stop.transform.position = lineSample.Position + Vector3.up;
            stop.transform.rotation = Quaternion.LookRotation(lineSample.Tangent);
            var barrier = stop.AddComponent<BoxCollider>(); barrier.size = new Vector3(4,2,0.1f); barrier.isTrigger = true;
            source.ConfigureRuntime(lights.Count + 1, stop);
            var line = signal.AddComponent<CityStopLine>(); line.Road = road; line.Distance = 22;
            signal.AddComponent<CityTrafficLightBridge>().Connect(source, line); lights.Add(source);
        }
        private void Update()
        {
            for (int i = 0; i < lights.Count; i++)
            {
                Color c = lights[i].LightState == TrafficLightState.Red ? Color.red : lights[i].LightState == TrafficLightState.Green ? Color.green : Color.yellow;
                lampColor.SetColor("_BaseColor", c); lampColor.SetColor("_Color", c); lamps[i].SetPropertyBlock(lampColor);
            }
        }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12,12,330,410), GUI.skin.box);
            GUILayout.Label("НОВЫЙ КОНТРОЛЛЕР С НУЛЯ");
            GUILayout.Label("Один цвет = один спавнер.\nСвои стоят в очереди, чужие сталкиваются.");
            if (!running && GUILayout.Button("Начать движение", GUILayout.Height(36)))
            { running = true; foreach (var spawner in spawners) spawner.enabled = true; }
            for (int i = 0; i < lights.Count; i++)
                if (GUILayout.Button(labels[i] + ": " + lights[i].LightState, GUILayout.Height(32))) lights[i].SwitchRedGreen();
            if (GUILayout.Button(Time.timeScale == 0 ? "Продолжить" : "Пауза")) Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            if (GUILayout.Button("Убрать машины"))
                for (int i = CityCarController.All.Count - 1; i >= 0; i--) Destroy(CityCarController.All[i].gameObject);
            int completed = 0; foreach (var spawner in spawners) completed += spawner.Completed;
            GUILayout.Label("Машин: " + CityCarController.All.Count + "   Проехали: " + completed);
            GUILayout.Label("Для проверки аварий включите пересекающиеся потоки одновременно.");
            GUILayout.EndArea();
        }
        private void OnDestroy()
        {
            Time.timeScale = 1;
            foreach (var material in new[] { asphalt, grass, white, pole }) if (material) Destroy(material);
        }
    }
}
