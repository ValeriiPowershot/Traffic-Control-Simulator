#if UNITY_EDITOR
using System.Collections;
using UnityEngine;

public sealed class OwnCarPlayRunner : MonoBehaviour
{
    private static BoxCollider carBody;
    public static void Begin()
    {
        UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);
        new GameObject("Actual Play Mode smoke test").AddComponent<OwnCarPlayRunner>();
        UnityEditor.EditorApplication.isPlaying = true;
    }
    private IEnumerator Start()
    {
        Physics.simulationMode = SimulationMode.FixedUpdate;
        Time.fixedDeltaTime = 0.02f;
        Time.timeScale = 8;
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.transform.position = new Vector3(0,-0.5f,0); ground.transform.localScale = new Vector3(100,1,100);
        var routeObject = new GameObject("Route"); routeObject.transform.position = new Vector3(0,0,100);
        var route = routeObject.AddComponent<RTC_Waypoint>(); route.desiredSpeedForNextWaypoint = 32;
        var car = CreateCar(route);
        var foreign = new GameObject("Foreign car"); foreign.SetActive(false); foreign.transform.position = new Vector3(0,0.6f,10);
        var other = foreign.AddComponent<ArcadeTrafficCar>(); other.enabled = false; other.StreamId = 99;
        foreign.GetComponent<Rigidbody>().isKinematic = true;
        foreign.AddComponent<BoxCollider>().size = new Vector3(1.5f,0.7f,3.6f);
        foreign.SetActive(true);
        float deadline = Time.time + 10;
        while (!car.HasCrashed && Time.time < deadline) yield return null;
        if (!car.HasCrashed) {Debug.LogError("OWN_PLAY_FAIL: real collision callback did not mark foreign crash; pos=" + car.transform.position + "; speed=" + car.SpeedKph + "; target=" + car.TargetSpeedKph + "; physics=" + Physics.simulationMode); UnityEditor.EditorApplication.Exit(1); yield break;}
        yield return new WaitForSeconds(1);
        foreach (var wheel in car.Wheels) if (wheel.Collider.motorTorque != 0) {Debug.LogError("OWN_PLAY_FAIL: motor still driving after crash"); UnityEditor.EditorApplication.Exit(1); yield break;}
        Debug.Log("OWN_PLAY_PASS: actual Play Mode foreign collision sets HasCrashed and removes motor torque; no RTC component on car");
        UnityEditor.EditorApplication.Exit(0);
    }
    private static ArcadeTrafficCar CreateCar(RTC_Waypoint route)
    {
        var go = new GameObject("Independent physical car"); go.SetActive(false); go.transform.position = new Vector3(0, 0.7f, 0);
        var rb = go.AddComponent<Rigidbody>(); rb.mass = 1200;
        carBody = go.AddComponent<BoxCollider>(); carBody.size = new Vector3(1.5f, 0.7f, 3.6f);
        var car = go.AddComponent<ArcadeTrafficCar>(); car.StreamId = 7; car.StartWaypoint = route;
        var com = new GameObject("COM"); com.transform.SetParent(go.transform, false); com.transform.localPosition = new Vector3(0, -0.3f, 0); car.CentreOfMass = com.transform;
        car.Wheels = new ArcadeTrafficCar.Wheel[4];
        for (int i = 0; i < 4; i++)
        {
            var wheel = new GameObject("Wheel"); wheel.transform.SetParent(go.transform, false); wheel.transform.localPosition = new Vector3(i % 2 == 0 ? -0.7f : 0.7f, -0.2f, i < 2 ? 1.2f : -1.2f);
            var wc = wheel.AddComponent<WheelCollider>(); wc.radius = 0.3f; wc.mass = 20; wc.suspensionDistance = 0.2f;
            var suspension = wc.suspensionSpring; suspension.spring = 35000; suspension.damper = 4500; suspension.targetPosition = 0.5f; wc.suspensionSpring = suspension;
            car.Wheels[i] = new ArcadeTrafficCar.Wheel {Collider = wc, Steering = i < 2, Driven = i >= 2};
        }
        go.SetActive(true); car.Initialize(); rb.interpolation = RigidbodyInterpolation.None; return car;
    }
}

#endif
