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
            ground.AddComponent<BoxCollider>().size = new Vector3(200,1,200);
            var curved = new GameObject("Own Bezier route").AddComponent<CityRoadPath>();
            curved.Curves = new[] {
                CityRoadPath.Curve.Straight(Vector3.zero, new Vector3(0,0,18), 28),
                new CityRoadPath.Curve {Start=new Vector3(0,0,18), HandleA=new Vector3(0,0,21.3137f), HandleB=new Vector3(2.6863f,0,24), End=new Vector3(6,0,24), SpeedKph=20},
                CityRoadPath.Curve.Straight(new Vector3(6,0,24), new Vector3(30,0,24),28)}; curved.Rebuild();
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
            Debug.Log("CLEAN_CORE_PASS: independent Bezier driving, narrow lane error=" + maxError + "; own queue; foreign contact; stop and release. No old vehicle assembly loaded.");
            EditorApplication.Exit(0);
        }
        catch (Exception e) {Debug.LogException(e); EditorApplication.Exit(1);}
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
