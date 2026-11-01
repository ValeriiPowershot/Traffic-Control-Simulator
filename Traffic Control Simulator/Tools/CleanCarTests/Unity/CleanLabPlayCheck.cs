#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using TrafficControl.CleanVehicles;

public sealed class CleanLabPlayCheck : MonoBehaviour
{
    public static void Begin()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/_Project/Sandbox/Scenes/ControllerTests/CleanCarControllerTest.unity");
        new GameObject("Clean lab play check").AddComponent<CleanLabPlayCheck>();
        UnityEditor.EditorApplication.isPlaying = true;
    }
    private IEnumerator Start()
    {
        Physics.simulationMode = SimulationMode.FixedUpdate;
        if (!Check(UnityEngine.Object.FindObjectOfType<CleanTrafficLab>() != null, "Saved clean scene loads its source script")) yield break;
        Time.timeScale = 6;
        yield return null;
        var spawns = UnityEngine.Object.FindObjectsOfType<CityCarSpawner>();
        foreach (var spawn in spawns) spawn.enabled = true;
        yield return new WaitForSeconds(9);
        if (!Check(CityCarController.All.Count >= 8, "Four new spawners form queues")) yield break;
        foreach (var car in CityCarController.All)
            if (!Check(car.GetComponents<MonoBehaviour>().Length == 2, "Car has only new controller and paint component")) yield break;
        var source = UnityEngine.Object.FindObjectsOfType<TrafficLight>()[0];
        var bridge = source.GetComponent<CityTrafficLightBridge>();
        source.SwitchRedGreen();
        yield return new WaitForSeconds(0.25f);
        if (!Check(source.LightState == TrafficLightState.Yellow && bridge.StopLine.Closed, "Original red-to-yellow still holds queue")) yield break;
        yield return new WaitForSeconds(2.1f);
        if (!Check(source.LightState == TrafficLightState.Green && !bridge.StopLine.Closed, "Original switching releases new controller")) yield break;
        yield return new WaitForSeconds(3);
        source.SwitchRedGreen();
        yield return new WaitForSeconds(0.2f);
        if (!Check(source.LightState == TrafficLightState.Yellow && !bridge.StopLine.Closed, "Original green-to-yellow remains open")) yield break;
        yield return new WaitForSeconds(2.1f);
        if (!Check(source.LightState == TrafficLightState.Red && bridge.StopLine.Closed, "Original red closes stop line")) yield break;

        var path = new GameObject("Foreign crash route").AddComponent<CityRoadPath>();
        path.Curves = new[] {CityRoadPath.Curve.Straight(new Vector3(-25,0,-30),new Vector3(-25,0,30))}; path.Rebuild();
        var leader = CityCarFactory.Create(path,22,202,Color.red); leader.Parked = true;
        var follower = CityCarFactory.Create(path,0,101,Color.blue);
        float deadline = Time.time + 12;
        while (follower.State != CityCarController.DrivingState.Crashed && Time.time < deadline) yield return null;
        if (!Check(follower.State == CityCarController.DrivingState.Crashed, "Actual foreign collision sets crash state")) yield break;
        yield return new WaitForSeconds(0.5f);
        if (!Check(follower.RearLeft.motorTorque == 0 && follower.RearRight.motorTorque == 0, "Crash removes motor torque")) yield break;
        foreach (var component in UnityEngine.Object.FindObjectsOfType<MonoBehaviour>())
            if (component.GetType().FullName.Contains("RTC_") || component.GetType().Name == "ArcadeTrafficCar")
            { Check(false,"Legacy vehicle component appeared in clean lab"); yield break; }
        Debug.Log("CLEAN_SAVED_SCENE_PASS: clean generated scene; original red/yellow/green coroutines; same-spawner queues; real foreign collision; zero legacy vehicle components.");
        UnityEditor.EditorApplication.Exit(0);
    }
    private static bool Check(bool ok,string message)
    {
        if (ok) Debug.Log("PASS " + message);
        else {Debug.LogError("CLEAN_LAB_PLAY_FAIL " + message); UnityEditor.EditorApplication.Exit(1);}
        return ok;
    }
}

#endif
