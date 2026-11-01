using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
    public sealed class CityCarController : MonoBehaviour
    {
        public enum DrivingState { Driving, Waiting, Crashed, Arrived, Invalid }
        private static readonly List<CityCarController> cars = new List<CityCarController>();
        public static IReadOnlyList<CityCarController> All => cars;
        public CityRoadPath Road;
        public BoxCollider Chassis;
        public WheelCollider FrontLeft, FrontRight, RearLeft, RearRight;
        public Transform[] WheelModels;
        [Header("Handling")]
        public float TopSpeedKph = 30;
        public float MaxAcceleration = 3, MaxDeceleration = 7, ComfortableBraking = 3;
        public float MaxLateralAcceleration = 3;
        public float MaxSteerDegrees = 45, SteerDegreesPerSecond = 220;
        public float LaneCorrection = 1.8f, AccelerationJerk = 8;
        [Header("Traffic")]
        public float QueueGap = 1.3f, ReactionSeconds = 0.45f;
        public bool Parked;
        public float CrashImpact = 2;
        public int OriginId { get; private set; }
        public DrivingState State { get; private set; }
        public float SpeedKph { get; private set; }
        public float TargetSpeedKph { get; private set; }
        public float RouteDistance { get; private set; }
        public float LaneError { get; private set; }
        public event Action<CityCarController> Arrived;
        public event Action<CityCarController, CityCarController> ForeignCollision;
        private Rigidbody body;
        private WheelCollider[] wheels;
        private float wheelbase, rearAxle, frontReach, rearReach, width, track;
        private float steeringAngle, acceleration;
        private bool ready, arrivalSent;
        private void Awake() { body = GetComponent<Rigidbody>(); }
        private void OnEnable() { if (!cars.Contains(this)) cars.Add(this); }
        private void OnDisable() { cars.Remove(this); HoldWheels(); }
        private void Start() { if (!ready) Prepare(Road, OriginId); }

        public bool Prepare(CityRoadPath road, int origin)
        {
            ready = false; body = GetComponent<Rigidbody>(); Road = road; OriginId = origin;
            wheels = new[] { FrontLeft, FrontRight, RearLeft, RearRight };
            if (!road || road.Length <= 0 || !Chassis || Array.Exists(wheels, w => w == null))
            { State = DrivingState.Invalid; HoldWheels(); Debug.LogError(name + ": assign a new CityRoadPath, chassis and four wheels.", this); return false; }
            rearAxle = (Local(RearLeft).z + Local(RearRight).z) * 0.5f;
            wheelbase = (Local(FrontLeft).z + Local(FrontRight).z) * 0.5f - rearAxle;
            track = Mathf.Abs(Local(FrontLeft).x - Local(FrontRight).x);
            if (wheelbase < 0.2f) { State = DrivingState.Invalid; HoldWheels(); return false; }
            frontReach = Chassis.center.z + Chassis.size.z * 0.5f - rearAxle;
            rearReach = rearAxle - (Chassis.center.z - Chassis.size.z * 0.5f);
            width = Chassis.size.x;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            RouteDistance = Road.Project(RearPosition(), 0, true);
            State = DrivingState.Driving; arrivalSent = false; acceleration = steeringAngle = 0;
            ready = true; return true;
        }
        private Vector3 Local(WheelCollider w) => transform.InverseTransformPoint(w.transform.position);
        private Vector3 RearPosition() => body.position + body.rotation * new Vector3(0, 0, rearAxle);
        private static Vector3 Flat(Vector3 v) { v.y = 0; return v; }

        private void FixedUpdate()
        {
            if (!ready) return;
            float dt = Time.fixedDeltaTime;
            float speed = Mathf.Max(0, Vector3.Dot(body.velocity, transform.forward));
            SpeedKph = Flat(body.velocity).magnitude * 3.6f;
            RouteDistance = Road.Project(RearPosition(), RouteDistance);
            var here = Road.At(RouteDistance);
            Vector3 right = Vector3.Cross(Vector3.up, here.Tangent).normalized;
            LaneError = Vector3.Dot(Flat(RearPosition() - here.Position), right);
            float heading = Vector3.SignedAngle(Flat(transform.forward), here.Tangent, Vector3.up) * Mathf.Deg2Rad;
            float wantedAngle = CityCarMath.RoadWheelAngle(heading, LaneError, speed, here.Curvature, wheelbase, LaneCorrection, MaxSteerDegrees * Mathf.Deg2Rad) * Mathf.Rad2Deg;
            steeringAngle = Mathf.MoveTowards(steeringAngle, wantedAngle, Mathf.Max(1, SteerDegreesPerSecond) * dt);

            float wantedSpeed = Mathf.Min(Mathf.Max(0, TopSpeedKph) / 3.6f, here.Speed);
            float preview = Mathf.Max(10, speed * speed / (2 * Mathf.Max(0.1f, ComfortableBraking)) + 5);
            for (float d = 0; d <= preview; d += 1)
            {
                var ahead = Road.At(RouteDistance + d);
                float cap = Mathf.Min(ahead.Speed, CityCarMath.BendSpeed(ahead.Curvature, MaxLateralAcceleration));
                wantedSpeed = Mathf.Min(wantedSpeed, Mathf.Sqrt(cap * cap + 2 * Mathf.Max(0.1f, ComfortableBraking) * Mathf.Max(0, d - 1)));
            }
            wantedSpeed = Mathf.Min(wantedSpeed, CityCarMath.SafeSpeed(Road.Length - RouteDistance - 0.15f, 0, ComfortableBraking, ReactionSeconds));
            bool emergency = false;
            for (int i = 0; i < cars.Count; i++)
            {
                var other = cars[i];
                if (!other || other == this || !other.ready || other.Road != Road || other.OriginId != OriginId) continue;
                if (other.RouteDistance <= RouteDistance || Mathf.Abs(other.LaneError) > (width + other.width) * 0.5f + 0.4f) continue;
                float gap = other.RouteDistance - RouteDistance - frontReach - other.rearReach;
                float leaderSpeed = Mathf.Max(0, Vector3.Dot(other.body.velocity, Road.At(other.RouteDistance).Tangent));
                wantedSpeed = Mathf.Min(wantedSpeed, CityCarMath.SafeSpeed(gap - QueueGap, leaderSpeed, ComfortableBraking, ReactionSeconds));
                if (gap < QueueGap * 0.5f) emergency = true;
            }
            var stops = CityStopLine.All;
            for (int i = 0; i < stops.Count; i++)
            {
                var line = stops[i];
                if (!line || line.Road != Road || !line.Closed) continue;
                float remaining = line.Distance - RouteDistance - frontReach;
                // Once the rear axle has passed a line, changing it cannot stop a car in the junction.
                if (line.Distance < RouteDistance) continue;
                wantedSpeed = Mathf.Min(wantedSpeed, CityCarMath.SafeSpeed(remaining - QueueGap, 0, ComfortableBraking, ReactionSeconds));
                if (remaining < QueueGap * 0.5f) emergency = true;
            }
            if (Parked || State == DrivingState.Crashed || State == DrivingState.Arrived) wantedSpeed = 0;
            TargetSpeedKph = wantedSpeed * 3.6f;
            if (State != DrivingState.Crashed && State != DrivingState.Arrived)
                State = wantedSpeed < 0.1f ? DrivingState.Waiting : DrivingState.Driving;
            float demand = Mathf.Clamp((wantedSpeed - speed) * 2.5f + (wantedSpeed > 0.5f ? 0.15f : 0), -MaxDeceleration, MaxAcceleration);
            if (wantedSpeed < 0.05f && speed < 0.25f) demand = -MaxDeceleration;
            acceleration = emergency || State == DrivingState.Crashed ? -MaxDeceleration : Mathf.MoveTowards(acceleration, demand, Mathf.Max(0.1f, AccelerationJerk) * dt);
            ApplyWheels();
            if (!arrivalSent && Road.Length - RouteDistance < 0.4f && SpeedKph < 0.5f && State != DrivingState.Crashed)
            { arrivalSent = true; State = DrivingState.Arrived; Arrived?.Invoke(this); }
        }

        private void ApplyWheels()
        {
            float angle = Mathf.Abs(steeringAngle) * Mathf.Deg2Rad;
            float radius = wheelbase / Mathf.Max(0.001f, Mathf.Tan(angle));
            float inner = angle < 0.00001f ? 0 : Mathf.Atan(wheelbase / Mathf.Max(0.3f, radius - track * 0.5f)) * Mathf.Rad2Deg;
            float outer = angle < 0.00001f ? 0 : Mathf.Atan(wheelbase / (radius + track * 0.5f)) * Mathf.Rad2Deg;
            float sign = Mathf.Sign(steeringAngle);
            FrontLeft.steerAngle = sign * Mathf.Min(60, steeringAngle < 0 ? inner : outer);
            FrontRight.steerAngle = sign * Mathf.Min(60, steeringAngle > 0 ? inner : outer);
            foreach (var w in wheels)
            {
                bool driven = w == RearLeft || w == RearRight;
                w.motorTorque = driven && acceleration > 0 && State != DrivingState.Crashed ? acceleration * body.mass * w.radius * 0.5f : 0;
                w.brakeTorque = acceleration < 0 ? -acceleration * body.mass * w.radius * 0.25f : 0;
            }
        }
        private void HoldWheels()
        {
            if (wheels == null) return;
            foreach (var w in wheels) if (w) { w.motorTorque = 0; w.brakeTorque = 10000; }
        }
        private void Update()
        {
            if (!ready || WheelModels == null) return;
            for (int i = 0; i < Mathf.Min(wheels.Length, WheelModels.Length); i++)
                if (WheelModels[i]) { wheels[i].GetWorldPose(out var p, out var r); WheelModels[i].SetPositionAndRotation(p, r); }
        }
        private void OnCollisionEnter(Collision contact)
        {
            var other = contact.collider.GetComponentInParent<CityCarController>();
            if (!other || other == this || other.OriginId == OriginId || contact.relativeVelocity.magnitude < CrashImpact) return;
            State = DrivingState.Crashed; acceleration = -MaxDeceleration; HoldWheels();
            ForeignCollision?.Invoke(this, other);
        }
    }
}
