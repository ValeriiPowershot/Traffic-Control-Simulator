using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Realistic_Traffic_Controller.Scripts;

public static class DrivingPhysicsCheck
{
    static readonly BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;
    public static void Run()
    {
        try
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Physics.simulationMode = SimulationMode.Script;
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.transform.position = new Vector3(0, -0.5f, 0);
            ground.transform.localScale = new Vector3(200, 1, 200);
            var a = Waypoint(new Vector3(0, 0, 18));
            var b = Waypoint(new Vector3(18, 0, 18));
            var c = Waypoint(new Vector3(18, 0, 42));
            a.nextWaypoint = b; b.previousWaypoint = a; b.nextWaypoint = c; c.previousWaypoint = b;
            var go = new GameObject("Physics test car");
            go.SetActive(false);
            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 1200;
            rb.interpolation = RigidbodyInterpolation.None;
            go.transform.position = new Vector3(0, 0.7f, 0);
            var body = go.AddComponent<BoxCollider>();
            body.size = new Vector3(1.5f, 0.7f, 3.6f);
            var car = go.AddComponent<RTC_CarController>();
            var com = new GameObject("COM"); com.transform.SetParent(go.transform, false); com.transform.localPosition = new Vector3(0, -0.3f, 0);
            car.COM = com.transform;
            car.wheels = new RTC_CarController.Wheel[4];
            for (int i = 0; i < 4; i++)
            {
                var w = new GameObject("Wheel" + i); w.transform.SetParent(go.transform, false);
                w.transform.localPosition = new Vector3(i % 2 == 0 ? -0.7f : 0.7f, -0.2f, i < 2 ? 1.2f : -1.2f);
                var wc = w.AddComponent<WheelCollider>();
                wc.radius = 0.3f; wc.suspensionDistance = 0.2f; wc.mass = 20;
                var spring = wc.suspensionSpring; spring.spring = 35000; spring.damper = 4500; spring.targetPosition = 0.5f; wc.suspensionSpring = spring;
                car.wheels[i] = new RTC_CarController.Wheel {wheelCollider = wc, isSteering = i < 2, isTraction = i >= 2, isBraking = true};
            }
            car.gearShiftUpRPM = 20000; car.gearShiftDownRPM = 0;
            car.engineTorque = 180; car.brakeTorque = 4000; car.maximumSpeed = 100;
            car.citySpeedLimit = 25; car.sounding = false; car.lighting = false;
            car.spawnPositionOffset = Vector3.zero;
            car.bounds.front = 1.8f; car.bounds.rear = -1.8f; car.bounds.left = -0.75f; car.bounds.right = 0.75f;
            car.bounds.up = 0.35f; car.bounds.down = -0.35f;
            car.currentWaypoint = a; car.nextWaypoint = a;
            car.CreateEngineTorqueCurve();
            go.SetActive(true);
            // Edit-mode simulation deliberately invokes lifecycle and fixed-step explicitly.
            Invoke(car, "Awake"); Invoke(car, "OnEnable");
            car.SetWaypoint(a);
            var trace = new System.Text.StringBuilder("t,x,z,speed,desired,steer,brake\n");
            float furthestZ = 0, maxX = 0;
            for (int i = 0; i < 2500; i++)
            {
                Invoke(car, "FixedUpdate");
                Physics.Simulate(0.02f);
                var pos = go.transform.position;
                if (i % 10 == 0) trace.AppendLine(string.Join(",", i * 0.02f, pos.x, pos.z, car.currentSpeed, car.desiredSpeed, car.steerInput, car.brakeInput));
                furthestZ = Mathf.Max(furthestZ, pos.z); maxX = Mathf.Max(maxX, pos.x);
                if (float.IsNaN(pos.x) || Mathf.Abs(pos.x) > 60 || Mathf.Abs(pos.z) > 80) throw new Exception("Unstable car position " + pos);
            }
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "driving-physics-trace.csv"), trace.ToString());
            string result = "Physical route: maxX=" + maxX + ", maxZ=" + furthestZ + ", final=" + go.transform.position;
            File.WriteAllText(Path.Combine(Path.GetTempPath(), "driving-physics-result.txt"), result);
            if (maxX < 15 || furthestZ < 35) throw new Exception("Did not complete both turns. " + result);
            // Validate the actual sensor code against child colliders and different spawn indices.
            go.transform.SetPositionAndRotation(new Vector3(0, 0.6f, 0), Quaternion.identity);
            rb.position = new Vector3(0, 0.6f, 0); rb.rotation = Quaternion.identity;
            rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
            var leader = new GameObject("Leader"); leader.SetActive(false);
            var lc = leader.AddComponent<RTC_CarController>(); lc.enabled = false; lc.CarSpawnIndex = 99;
            leader.transform.position = new Vector3(0, 0.6f, 5);
            var child = new GameObject("Child collider"); child.transform.SetParent(leader.transform, false);
            child.AddComponent<BoxCollider>().size = new Vector3(1.5f, 0.7f, 3.6f);
            leader.SetActive(true); Physics.SyncTransforms();
            float queueLimit = Sense(car);
            if (!car.stoppedForVehicle || queueLimit >= 8) throw new Exception("Queue detection failed limit=" + queueLimit + ", flag=" + car.stoppedForVehicle + ", car=" + go.transform.position + ", leader=" + leader.transform.position);
            UnityEngine.Object.DestroyImmediate(leader);
            var signal = new GameObject("Red signal"); signal.transform.position = new Vector3(0, 0.6f, 8);
            var light = signal.AddComponent<TrafficLight>(); light.TrafficLightSpawnIndex = car.CarSpawnIndex;
            var barrier = signal.AddComponent<BoxCollider>(); barrier.size = new Vector3(2, 1, 0.2f);
            Physics.SyncTransforms();
            float redLimit = Sense(car);
            if (!car.stoppedForTrafficLight || redLimit >= 8) throw new Exception("Red signal was ignored");
            typeof(TrafficLight).GetField("<LightState>k__BackingField", Flags).SetValue(light, TrafficLightState.Green);
            barrier.enabled = false; Physics.SyncTransforms();
            float greenLimit = Sense(car);
            if (car.stoppedForTrafficLight || greenLimit < 7.99f) throw new Exception("Green signal did not release car");
            // Stop and restart with real WheelCollider simulation, not just sensor outputs.
            typeof(TrafficLight).GetField("<LightState>k__BackingField", Flags).SetValue(light, TrafficLightState.Red);
            barrier.enabled = true; signal.transform.position = new Vector3(0, 0.6f, 12);
            go.transform.SetPositionAndRotation(new Vector3(0, 0.7f, 0), Quaternion.identity);
            rb.position = go.transform.position; rb.rotation = Quaternion.identity;
            Invoke(car, "ResetVehicleState"); car.SetWaypoint(a); Physics.SyncTransforms();
            for (int i = 0; i < 1000; i++) {Invoke(car, "FixedUpdate"); Physics.Simulate(0.02f);}
            float redStopZ = go.transform.position.z;
            if (redStopZ < 3 || redStopZ > 10 || car.currentSpeed > 0.3f)
                throw new Exception("Physical red stop failed z=" + redStopZ + ", speed=" + car.currentSpeed);
            typeof(TrafficLight).GetField("<LightState>k__BackingField", Flags).SetValue(light, TrafficLightState.Green);
            barrier.enabled = false; Physics.SyncTransforms();
            for (int i = 0; i < 250; i++) {Invoke(car, "FixedUpdate"); Physics.Simulate(0.02f);}
            if (go.transform.position.z < redStopZ + 2) throw new Exception("Physical green restart failed");
            Debug.Log("DRIVING_PHYSICS_PASS " + result + "; child collider across spawners; physical red stop z=" + redStopZ + "; green restart passed");
            EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
    static float Sense(RTC_CarController car) {return (float)typeof(RTC_CarController).GetMethod("SenseDrivingObstacles", Flags).Invoke(car, new object[]{8f, 8f, new Vector3(0,0,15)});}
    static RTC_Waypoint Waypoint(Vector3 p) {var g = new GameObject("Waypoint"); g.transform.position = p; var w = g.AddComponent<RTC_Waypoint>(); w.desiredSpeedForNextWaypoint = 25; return w;}
    static void Invoke(RTC_CarController car, string name) {typeof(RTC_CarController).GetMethod(name, Flags).Invoke(car, null);}
}
