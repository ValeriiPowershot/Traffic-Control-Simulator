using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using TrafficControl.CleanVehicles;

public static class CleanCarCheck
{
    private static readonly BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly MethodInfo tick = typeof(CityCarController).GetMethod("FixedUpdate", flags);
    public static void Run()
    {
        try
        {
            Physics.simulationMode = SimulationMode.Script;
            var ground = new GameObject("Floor"); ground.transform.position = new Vector3(0,-0.5f,0);
            ground.AddComponent<BoxCollider>().size = new Vector3(1000,1,1000);
            CheckSurveyRoute(1, new[] {new Vector3(121.8f,0,86.3f), new Vector3(156.30582f,0,86.28656f), new Vector3(191.86f,0,86.27545f), new Vector3(341.09998f,0,85.2f)});
            CheckSurveyRoute(2, new[] {new Vector3(322.7801f,0,92.913574f), new Vector3(245.87715f,0,92.92146f), new Vector3(208.41486f,0,92.92465f), new Vector3(95.5f,0,93.02463f)});
            CheckSurveyRoute(3, new[] {new Vector3(229.3f,0,13.389999f), new Vector3(228.40001f,0,42.4f), new Vector3(227.84001f,0,85.87f), new Vector3(244.51f,0,86.123f), new Vector3(311.69998f,0,86.35f)});
            CheckSurveyRoute(4, new[] {new Vector3(181.7f,0,216.0f), new Vector3(181.69998f,0,149.9f), new Vector3(182.56f,0,97.32001f), new Vector3(173.88f,0,92.520004f), new Vector3(97.600006f,0,92.7f)});
            var curved = new GameObject("Own Bezier route").AddComponent<CityRoadPath>();
            curved.Curves = new[] {
                CityRoadPath.Curve.Straight(Vector3.zero, new Vector3(0,0,18), 28),
                new CityRoadPath.Curve {Start=new Vector3(0,0,18), HandleA=new Vector3(0,0,21.3137f), HandleB=new Vector3(2.6863f,0,24), End=new Vector3(6,0,24), SpeedKph=20},
                CityRoadPath.Curve.Straight(new Vector3(6,0,24), new Vector3(30,0,24),28)}; curved.Rebuild();
            // Authoring corners preserve position and tangent at each seam.
            var built = CityRouteBuilder.Build(new[] { Vector3.zero, new Vector3(0,0,20), new Vector3(20,0,20) });
            for (int i = 1; i < built.Length; i++)
            {
                Check(Vector3.Distance(built[i-1].End, built[i].Start) < 0.001f, "Route joins have no gaps");
                Check(Vector3.Angle(built[i-1].End-built[i-1].HandleB, built[i].HandleA-built[i].Start) < 0.1f, "Route joins share a tangent");
            }
            var car = Spawn(curved, 0, 101);
            float maxError = 0;
            for (int i = 0; i < 2200; i++) {Step(car); maxError = Mathf.Max(maxError, Mathf.Abs(car.LaneError));}
            Check(car.State == CityCarController.DrivingState.Arrived, "Complete curved route: " + car.transform.position + "; distance=" + car.RouteDistance + "; state=" + car.State);
            Check(maxError < 0.85f, "Keep rear axle inside narrow lane: " + maxError);
            UnityEngine.Object.DestroyImmediate(car.gameObject);

            var straight = new GameObject("Queue road").AddComponent<CityRoadPath>();
            straight.Curves = new[] {CityRoadPath.Curve.Straight(Vector3.zero, new Vector3(0,0,100),30)}; straight.Rebuild();
            var leader = Spawn(straight, 22, 101); leader.Parked = true; leader.GetComponent<Rigidbody>().isKinematic = true;
            car = Spawn(straight, 0, 101);
            for (int i = 0; i < 1500; i++) {tick.Invoke(leader,null); Step(car);}
            float gap = leader.Chassis.bounds.min.z - car.Chassis.bounds.max.z;
            Check(car.SpeedKph < 0.3f && gap > 0.7f && gap < 2.5f, "Own spawner queue gap=" + gap + "; speed=" + car.SpeedKph);
            UnityEngine.Object.DestroyImmediate(leader.gameObject); UnityEngine.Object.DestroyImmediate(car.gameObject);

            leader = Spawn(straight, 22, 202); leader.Parked = true; leader.GetComponent<Rigidbody>().isKinematic = true;
            car = Spawn(straight, 0, 101);
            bool contacted = false; float approach = 0;
            for (int i = 0; i < 800; i++)
            {
                tick.Invoke(leader,null); Step(car);
                if (car.RouteDistance > 14 && approach == 0) approach = car.SpeedKph;
                if (leader.Chassis.bounds.min.z - car.Chassis.bounds.max.z < 0.04f) { contacted = true; break; }
            }
            Check(contacted && approach > 12, "Foreign spawner remains collidable; approach=" + approach);
            UnityEngine.Object.DestroyImmediate(leader.gameObject); UnityEngine.Object.DestroyImmediate(car.gameObject);

            var stop = new GameObject("Independent stop line").AddComponent<CityStopLine>(); stop.Road = straight; stop.Distance = 22;
            typeof(CityStopLine).GetMethod("OnEnable",flags).Invoke(stop,null);
            car = Spawn(straight, 0, 101);
            for (int i = 0; i < 1500; i++) Step(car);
            float stopped = car.RouteDistance;
            Check(stopped > 10 && stopped < 19 && car.SpeedKph < 0.3f, "Closed line stops car: " + stopped);
            stop.ManualClosed = false;
            for (int i = 0; i < 250; i++) Step(car);
            Check(car.RouteDistance > stopped + 4, "Opening line releases queue");
            // Actual physical occupation blocks even a different stream at spawn.
            UnityEngine.Object.DestroyImmediate(car.gameObject);
            var spawner = new GameObject("Guarded spawn").AddComponent<CityCarSpawner>();
            spawner.Road = straight; spawner.StartAutomatically = false;
            leader = Spawn(straight, 0, 999); leader.Parked = true; Physics.SyncTransforms();
            Check(spawner.Spawn() == null, "Foreign car occupying spawn blocks creation");
            UnityEngine.Object.DestroyImmediate(leader.gameObject);
            var created = spawner.Spawn();
            Check(created != null, "Clear spawn creates car");
            Check(spawner.Spawn() == null, "Immediate second spawn cannot overlap first");
            UnityEngine.Object.DestroyImmediate(created.gameObject);
            foreach (var vehicle in new System.Collections.Generic.List<CityCarController>(CityCarController.All))
                if (vehicle) UnityEngine.Object.DestroyImmediate(vehicle.gameObject);
            // Release delay must hold brakes, then allow forward motion.
            stop.ManualClosed = true;
            car = Spawn(straight, 0, 101); car.StartReactionSeconds = 0.6f;
            for (int i = 0; i < 1200; i++) Step(car);
            float before = car.RouteDistance; stop.ManualClosed = false;
            for (int i = 0; i < 15; i++) Step(car);
            Check(car.TargetSpeedKph == 0 && car.RearLeft.motorTorque == 0, "Reaction delay keeps motor off");
            for (int i = 0; i < 200; i++) Step(car);
            Check(car.RouteDistance > before + 3, "Queue releases after finite reaction delay: before=" + before + "; now=" + car.RouteDistance + "; speed=" + car.SpeedKph + "; target=" + car.TargetSpeedKph + "; state=" + car.State);
            Debug.Log("CLEAN_CORE_PASS: independent Bezier driving, narrow lane error=" + maxError + "; own queue; foreign contact; stop and release. No old vehicle assembly loaded.");
            EditorApplication.Exit(0);
        }
        catch (Exception e) {Debug.LogException(e); EditorApplication.Exit(1);}
    }
    private static void CheckSurveyRoute(int stream, Vector3[] points)
    {
        var road = new GameObject("Level 1 survey " + stream).AddComponent<CityRoadPath>();
        road.Curves = CityRouteBuilder.Build(points); road.Rebuild();
        var car = Spawn(road, 0, stream); float error = 0;
        for (int i = 0; i < 4500 && car.State != CityCarController.DrivingState.Arrived; i++)
        { Step(car); error = Mathf.Max(error, Mathf.Abs(car.LaneError)); }
        Check(car.State == CityCarController.DrivingState.Arrived && error < 0.85f,
            "Level 1 stream " + stream + ": state=" + car.State + "; lane error=" + error);
        UnityEngine.Object.DestroyImmediate(car.gameObject); UnityEngine.Object.DestroyImmediate(road.gameObject);
    }
    private static void Check(bool ok,string message) {if (!ok) throw new Exception(message); Debug.Log("PASS " + message);}
    private static CityCarController Spawn(CityRoadPath road,float distance,int owner)
    {
        var car = CityCarFactory.Create(road,distance,owner,Color.cyan);
        typeof(CityCarController).GetMethod("OnEnable", flags).Invoke(car,null);
        car.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
        Physics.SyncTransforms(); return car;
    }
    private static void Step(CityCarController car) {tick.Invoke(car,null); Physics.Simulate(0.02f);}
}
