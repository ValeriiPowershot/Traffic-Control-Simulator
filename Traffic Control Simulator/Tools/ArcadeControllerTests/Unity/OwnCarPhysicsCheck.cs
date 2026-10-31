using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class OwnCarPhysicsCheck
{
    private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
    private static BoxCollider carBody;
    public static void Run()
    {
        try
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Physics.simulationMode = SimulationMode.Script;
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.transform.position = new Vector3(0, -0.5f, 0); ground.transform.localScale = new Vector3(200, 1, 200);
            var a = Point(new Vector3(0, 0, 18)); var b = Point(new Vector3(18, 0, 18)); var c = Point(new Vector3(18, 0, 45));
            a.nextWaypoint = b; b.nextWaypoint = c;
            var car = CreateCar(a);
            Step(car, 2500);
            Check(car.transform.position.x > 16 && car.transform.position.z > 42 && car.SpeedKph < 0.2f, "Physical left/right turns and final stop: " + car.transform.position);

            var straight = Point(new Vector3(0, 0, 100));
            var leaderObject = new GameObject("Own stream leader"); leaderObject.SetActive(false);
            leaderObject.transform.position = new Vector3(0, 0.6f, 12);
            var leader = leaderObject.AddComponent<ArcadeTrafficCar>(); leader.enabled = false; leader.StreamId = 7;
            leaderObject.GetComponent<Rigidbody>().isKinematic = true;
            var child = new GameObject("Child body"); child.transform.SetParent(leaderObject.transform, false);
            var leaderBody = child.AddComponent<BoxCollider>(); leaderBody.size = new Vector3(1.5f, 0.7f, 3.6f);
            leaderObject.SetActive(true);
            Reset(car, straight);
            Step(car, 1200);
            float ownStop = car.transform.position.z;
            Check(ownStop > 3 && ownStop < 8.5f && car.SpeedKph < 0.2f, "Stop before own-stream car: z=" + ownStop + " speed=" + car.SpeedKph);
            Check(leaderBody.bounds.min.z - carBody.bounds.max.z > 0.2f, "Own cars must stop without contact");

            // Identical geometry, different stream: the driver must not slow pre-emptively.
            leader.StreamId = 99;
            Reset(car, straight);
            bool reachedForeign = false;
            float approachSpeed = 0;
            for (int i = 0; i < 600; i++)
            {
                Step(car, 1);
                if (car.transform.position.z > 6 && approachSpeed == 0) approachSpeed = car.SpeedKph;
                if (leaderBody.bounds.min.z - carBody.bounds.max.z < 0.04f) { reachedForeign = true; break; }
            }
            Check(approachSpeed > 8, "Must not brake in advance for foreign stream: " + approachSpeed);
            Check(reachedForeign, "Foreign car must remain collidable");
            UnityEngine.Object.DestroyImmediate(leaderObject);

            var signalObject = new GameObject("Test traffic light"); signalObject.transform.position = new Vector3(0, 0.6f, 12);
            var signal = signalObject.AddComponent<TrafficLight>(); signal.enabled = false; signal.TrafficLightSpawnIndex = 7;
            var stop = new GameObject("Stop barrier"); stop.transform.SetParent(signalObject.transform, false);
            var stopCollider = stop.AddComponent<BoxCollider>(); stopCollider.isTrigger = true; stopCollider.size = new Vector3(2, 1, 0.2f);
            typeof(TrafficLight).GetField("_stopCollider", Flags).SetValue(signal, stop);
            typeof(TrafficLight).GetMethod("OnEnable", Flags).Invoke(signal, null);
            State(signal, TrafficLightState.Red);
            Reset(car, straight); Step(car, 1200);
            float redStop = car.transform.position.z;
            Check(redStop > 3 && redStop < 10 && car.SpeedKph < 0.2f, "Red light stop: " + redStop);
            State(signal, TrafficLightState.Yellow); Step(car, 200);
            Check(Mathf.Abs(car.transform.position.z - redStop) < 0.1f, "Wait on yellow after red");
            State(signal, TrafficLightState.Green); stop.SetActive(false); Step(car, 250);
            Check(car.transform.position.z > redStop + 3, "Move on green");
            State(signal, TrafficLightState.Yellow);
            Check(!signal.BlocksTraffic, "Yellow after green preserves open barrier");
            signal.TrafficLightSpawnIndex = 99; State(signal, TrafficLightState.Red); stop.SetActive(true);
            Reset(car, straight); Step(car, 200);
            Check(car.transform.position.z > 12, "Ignore another stream's red light");
            string result = "OWN_CONTROLLER_PHYSICS_PASS: two bends; end stop; own queue without contact z=" + ownStop + "; foreign approach speed=" + approachSpeed + "; red stop z=" + redStop + "; yellow and green; foreign signal ignored.";
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "own-car-physics-result.txt"), result);
            Debug.Log(result); EditorApplication.Exit(0);
        }
        catch (Exception e) {Debug.LogException(e); EditorApplication.Exit(1);}
    }
    private static void Check(bool success, string message) {if (!success) throw new Exception(message); Debug.Log("PASS " + message);}
    private static RTC_Waypoint Point(Vector3 position) {var go = new GameObject("Route"); go.transform.position = position; var w = go.AddComponent<RTC_Waypoint>(); w.desiredSpeedForNextWaypoint = 32; return w;}
    private static void State(TrafficLight signal, TrafficLightState state) {typeof(TrafficLight).GetField("<LightState>k__BackingField", Flags).SetValue(signal, state); Physics.SyncTransforms();}
    private static void Step(ArcadeTrafficCar car, int count) {for (int i = 0; i < count; i++) {typeof(ArcadeTrafficCar).GetMethod("FixedUpdate", Flags).Invoke(car, null); Physics.Simulate(0.02f);}}
    private static void Reset(ArcadeTrafficCar car, RTC_Waypoint route)
    {
        car.transform.SetPositionAndRotation(new Vector3(0, 0.7f, 0), Quaternion.identity);
        var rb = car.GetComponent<Rigidbody>(); rb.position = car.transform.position; rb.rotation = Quaternion.identity; rb.velocity = rb.angularVelocity = Vector3.zero;
        car.StartWaypoint = route; car.Initialize(); rb.interpolation = RigidbodyInterpolation.None; Physics.SyncTransforms();
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
