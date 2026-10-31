using System;
using UnityEngine;
using Realistic_Traffic_Controller.Scripts;
using BaseCode.Logic.City;

[DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
public sealed class ArcadeTrafficCar : MonoBehaviour
{
    [Serializable]
    public sealed class Wheel
    {
        public WheelCollider Collider;
        public Transform Model;
        public bool Steering, Driven = true, Braking = true;
    }

    [Header("Vehicle")]
    public Wheel[] Wheels;
    public Transform CentreOfMass;
    public int StreamId;
    public RTC_Waypoint StartWaypoint;
    [Header("Physical arcade handling")]
    [Min(1)] public float SpeedLimitKph = 32;
    [Range(20, 60)] public float MaxSteeringAngle = 48;
    [Min(0.1f)] public float Acceleration = 3;
    [Min(0.1f)] public float BrakingDeceleration = 6;
    [Min(0.1f)] public float PlanningDeceleration = 3;
    [Min(0.1f)] public float LateralAcceleration = 3;
    [Min(0.1f)] public float SteeringRate = 5;
    [Min(0)] public float GripAssist = 1.5f;
    [Min(0)] public float AntiRollStrength = 2500;
    [Header("Traffic")]
    [Min(0.1f)] public float StandstillGap = 1.1f;
    [Min(0.1f)] public float FollowingTime = 1;
    public LayerMask DetectionMask = -1;
    [Min(0.1f)] public float ForeignCrashImpact = 2;
    public bool StopAfterForeignCrash = true;
    [Header("Diagnostics")]
    public bool DrawPath = true;
    public bool HasCrashed { get; private set; }
    public bool WaitingForOwnCar { get; private set; }
    public bool WaitingForLight { get; private set; }
    public float SpeedKph { get; private set; }
    public float TargetSpeedKph { get; private set; }
    public float SteeringInput { get; private set; }
    public string ConfigurationError { get; private set; }

    private Rigidbody body;
    private RTC_CarController appearance;
    private RTC_Waypoint waypoint, next;
    private readonly ArcadeTrafficPath path = new ArcadeTrafficPath();
    private readonly RaycastHit[] hits = new RaycastHit[64];
    private readonly Collider[] overlaps = new Collider[32];
    private Vector3 segmentStart, target;
    private float wheelbase, rearZ, frontZ, halfWidth, sensorY, wait, throttle, brake;
    private bool initialized;
    private int drivenCount, brakeCount;

    private void Awake() { body = GetComponent<Rigidbody>(); }
    private void Start() { if (!initialized) Initialize(); }

    // RTC is an optional import adapter: no RTC navigation, engine or input code runs afterwards.
    public static ArcadeTrafficCar TakeOver(RTC_CarController source, RTC_Waypoint route, int stream)
    {
        var driver = source.GetComponent<ArcadeTrafficCar>() ?? source.gameObject.AddComponent<ArcadeTrafficCar>();
        driver.appearance = source;
        driver.Wheels = new Wheel[source.wheels != null ? source.wheels.Length : 0];
        for (int i = 0; i < driver.Wheels.Length; i++)
        {
            var w = source.wheels[i];
            if (w == null) continue;
            driver.Wheels[i] = new Wheel { Collider = w.wheelCollider, Model = w.wheelModel, Steering = w.isSteering, Driven = w.isTraction, Braking = w.isBraking };
        }
        driver.CentreOfMass = source.COM;
        driver.StreamId = stream;
        source.CarSpawnIndex = stream;
        source.enabled = false;
        driver.StartWaypoint = route;
        driver.Initialize();
        return driver;
    }

    public void Initialize()
    {
        body = GetComponent<Rigidbody>();
        initialized = false;
        ConfigurationError = null;
        drivenCount = brakeCount = 0;
        float front = float.MinValue, rear = float.MaxValue;
        halfWidth = 0.5f;
        bool steers = false;
        if (Wheels != null) foreach (var w in Wheels)
        {
            if (w == null || !w.Collider) continue;
            Vector3 position = transform.InverseTransformPoint(w.Collider.transform.position);
            halfWidth = Mathf.Max(halfWidth, Mathf.Abs(position.x));
            if (w.Steering) { front = Mathf.Max(front, position.z); steers = true; }
            else rear = Mathf.Min(rear, position.z);
            if (w.Driven) drivenCount++;
            if (w.Braking) brakeCount++;
        }
        if (!steers || front <= rear || drivenCount == 0 || brakeCount == 0)
        {
            ConfigurationError = "Assign front steering wheels, rear wheels, drive wheels and brakes.";
            ApplyFailSafeBrakes();
            Debug.LogError(name + ": " + ConfigurationError, this);
            return;
        }
        wheelbase = front - rear;
        rearZ = rear;
        frontZ = front + 0.5f;
        sensorY = 0;
        if (appearance != null)
        {
            frontZ = Mathf.Max(frontZ, appearance.bounds.front);
            halfWidth = Mathf.Max(halfWidth, Mathf.Abs(appearance.bounds.right));
            sensorY = Mathf.Lerp(appearance.bounds.down, appearance.bounds.up, 0.55f);
        }
        if (CentreOfMass) body.centerOfMass = transform.InverseTransformPoint(CentreOfMass.position);
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        HasCrashed = false;
        wait = throttle = brake = 0;
        SteeringInput = 0;
        SetRoute(StartWaypoint, StreamId);
        initialized = true;
    }

    public void SetRoute(RTC_Waypoint start, int stream)
    {
        StreamId = stream;
        waypoint = start;
        next = ChooseNext(start);
        segmentStart = transform.position;
        if (appearance) appearance.CarSpawnIndex = stream;
    }

    private RTC_Waypoint ChooseNext(RTC_Waypoint from)
    {
        if (!from) return null;
        var branch = from.interConnectionWaypoint;
        if (branch && branch != from && (!from.nextWaypoint || UnityEngine.Random.Range(0, 3) == 1))
        {
            if (!appearance || !branch.connectedLane || RTC.EqualVehicleType(appearance.vehicleType, branch.connectedLane)) return branch;
        }
        return from.nextWaypoint != from ? from.nextWaypoint : null;
    }

    private void BuildPath()
    {
        float radius = wheelbase / Mathf.Tan(Mathf.Clamp(MaxSteeringAngle, 1, 70) * Mathf.Deg2Rad);
        path.Build(segmentStart, waypoint.transform.position, next ? next.transform.position : (Vector3?)null, radius);
    }

    private void FixedUpdate()
    {
        if (!initialized) return;
        float dt = Time.fixedDeltaTime;
        float speed = ArcadeTrafficPath.Flat(body.velocity).magnitude;
        SpeedKph = speed * 3.6f;
        Vector3 axle = transform.TransformPoint(new Vector3(0, 0, rearZ));
        wait = Mathf.Max(0, wait - dt);
        if (waypoint && wait <= 0 && !HasCrashed)
        {
            for (int i = 0; i < 4 && waypoint; i++)
            {
                BuildPath();
                Vector3 toExit = ArcadeTrafficPath.Flat(axle - path.Exit);
                bool passed = path.ExitDirection.sqrMagnitude > 0.001f && Vector3.Dot(toExit, path.ExitDirection) >= 0;
                bool close = path.Trim < 0.05f && toExit.magnitude < Mathf.Min(0.6f, waypoint.radius);
                if (!passed && !close) break;
                wait = Mathf.Max(0, waypoint.wait);
                segmentStart = waypoint.transform.position;
                waypoint = next;
                next = ChooseNext(waypoint);
                if (wait > 0) break;
            }
        }
        float wanted = 0;
        float steering = 0;
        if (waypoint && !HasCrashed)
        {
            BuildPath();
            target = path.Target(axle, Mathf.Clamp(1.5f + speed * 0.2f, 1, 2.8f));
            Vector3 local = transform.InverseTransformDirection(ArcadeTrafficPath.Flat(target - axle));
            steering = ArcadeTrafficRules.Steering(local.x, local.z, wheelbase, MaxSteeringAngle * Mathf.Deg2Rad);
            if (local.z < -0.1f) steering = local.x >= 0 ? 1 : -1;
            wanted = Mathf.Min(Mathf.Max(0, SpeedLimitKph), Mathf.Max(0, waypoint.desiredSpeedForNextWaypoint)) / 3.6f;
            float curvature = Mathf.Abs(Mathf.Tan(steering * MaxSteeringAngle * Mathf.Deg2Rad)) / wheelbase;
            wanted = Mathf.Min(wanted, Mathf.Sqrt(Mathf.Max(0.1f, LateralAcceleration) / Mathf.Max(0.0001f, curvature)));
            float distance = ArcadeTrafficPath.Flat(waypoint.transform.position - axle).magnitude;
            if (path.Curvature > 0.001f)
            {
                float curveSpeed = Mathf.Sqrt(Mathf.Max(0.1f, LateralAcceleration) / path.Curvature);
                wanted = Mathf.Min(wanted, ArcadeTrafficRules.ApproachSpeed(curveSpeed, Mathf.Max(0, distance - path.Trim - wheelbase), PlanningDeceleration));
            }
            if (!next) wanted = Mathf.Min(wanted, ArcadeTrafficRules.ApproachSpeed(0, Mathf.Max(0, distance - 0.5f), PlanningDeceleration));
            wanted = LimitForTraffic(wanted, speed);
        }
        if (wait > 0 || HasCrashed) wanted = 0;
        TargetSpeedKph = wanted * 3.6f;
        SteeringInput = Mathf.MoveTowards(SteeringInput, steering, Mathf.Max(0.1f, SteeringRate) * dt);
        float error = wanted - speed;
        float wantedBrake = wanted < 0.05f ? 1 : Mathf.Clamp01(-error / 1.5f);
        brake = Mathf.MoveTowards(brake, wantedBrake, 8 * dt);
        throttle = brake > 0.01f ? 0 : Mathf.MoveTowards(throttle, Mathf.Clamp01(error / 2), 2 * dt);
        DriveWheels();
        ApplyGrip();
        SyncAppearance();
    }

    private float LimitForTraffic(float wanted, float speed)
    {
        WaitingForOwnCar = WaitingForLight = false;
        Vector3 origin = transform.TransformPoint(new Vector3(0, sensorY, frontZ));
        Vector3 direction = ArcadeTrafficPath.Flat(transform.forward).normalized;
        float range = Mathf.Max(8, speed * FollowingTime + speed * speed / (2 * Mathf.Max(0.1f, PlanningDeceleration)) + StandstillGap);
        float width = Mathf.Max(0.15f, halfWidth * 0.65f);
        int count = Physics.SphereCastNonAlloc(origin, width, direction, hits, range, DetectionMask, QueryTriggerInteraction.Ignore);
        if (count == hits.Length) wanted = 0;
        for (int i = 0; i < count; i++) wanted = LimitForCar(hits[i].collider, hits[i].distance, direction, wanted);
        Vector3 curveDirection = ArcadeTrafficPath.Flat(target - origin).normalized;
        if (Vector3.Dot(curveDirection, direction) > 0.1f)
        {
            count = Physics.SphereCastNonAlloc(origin, width, curveDirection, hits, range, DetectionMask, QueryTriggerInteraction.Ignore);
            if (count == hits.Length) wanted = 0;
            for (int i = 0; i < count; i++) wanted = LimitForCar(hits[i].collider, hits[i].distance, curveDirection, wanted);
        }
        count = Physics.OverlapSphereNonAlloc(origin, width, overlaps, DetectionMask, QueryTriggerInteraction.Ignore);
        if (count == overlaps.Length) wanted = 0;
        for (int i = 0; i < count; i++) wanted = LimitForCar(overlaps[i], 0, direction, wanted);

        var signals = TrafficLight.ActiveSignals;
        for (int i = 0; i < signals.Count; i++)
        {
            var signal = signals[i];
            if (!signal || signal.gameObject.scene != gameObject.scene || signal.TrafficLightSpawnIndex != StreamId || !signal.BlocksTraffic) continue;
            var barrier = signal.StopBarrier;
            if (!barrier) continue;
            Vector3 delta = ArcadeTrafficPath.Flat(barrier.bounds.center - origin);
            float ahead = Vector3.Dot(delta, direction);
            if (ahead < -0.3f || ahead > range + 3) continue;
            float side = (delta - direction * ahead).magnitude;
            float barrierWidth = Mathf.Max(barrier.bounds.extents.x, barrier.bounds.extents.z);
            if (side > barrierWidth + halfWidth) continue;
            float gap = ArcadeTrafficPath.Flat(barrier.ClosestPoint(origin) - origin).magnitude;
            wanted = Mathf.Min(wanted, ArcadeTrafficRules.ApproachSpeed(0, Mathf.Max(0, gap - StandstillGap - speed * 0.15f), PlanningDeceleration));
            WaitingForLight = true;
        }
        return wanted;
    }

    private float LimitForCar(Collider collider, float gap, Vector3 direction, float wanted)
    {
        if (!collider || collider.transform.IsChildOf(transform)) return wanted;
        var other = collider.GetComponentInParent<ArcadeTrafficCar>();
        Rigidbody leader;
        int stream;
        if (other) { stream = other.StreamId; leader = other.body ? other.body : other.GetComponent<Rigidbody>(); }
        else
        {
            var rtc = collider.GetComponentInParent<RTC_CarController>();
            if (!rtc) return wanted;
            stream = rtc.CarSpawnIndex; leader = rtc.Rigid;
        }
        // Foreign traffic remains physically solid but is deliberately ignored by the driver.
        if (!ArcadeTrafficRules.ShouldFollow(StreamId, stream)) return wanted;
        WaitingForOwnCar = true;
        float leaderSpeed = leader ? Mathf.Max(0, Vector3.Dot(ArcadeTrafficPath.Flat(leader.velocity), direction)) : 0;
        return Mathf.Min(wanted, ArcadeTrafficRules.FollowSpeed(leaderSpeed, gap, StandstillGap, FollowingTime, PlanningDeceleration));
    }

    private void DriveWheels()
    {
        if (Wheels == null) return;
        foreach (var w in Wheels)
        {
            if (w == null || !w.Collider) continue;
            w.Collider.steerAngle = w.Steering ? SteeringInput * MaxSteeringAngle : 0;
            w.Collider.motorTorque = w.Driven && !HasCrashed ? throttle * Mathf.Max(0, Acceleration) * body.mass * w.Collider.radius / Mathf.Max(1, drivenCount) : 0;
            w.Collider.brakeTorque = w.Braking ? brake * Mathf.Max(0.1f, BrakingDeceleration) * body.mass * w.Collider.radius / Mathf.Max(1, brakeCount) : 0;
        }
    }

    private void ApplyGrip()
    {
        bool grounded = false;
        foreach (var w in Wheels) if (w != null && w.Collider && w.Collider.isGrounded) grounded = true;
        if (!grounded || HasCrashed) return;
        float lateral = Vector3.Dot(body.velocity, transform.right);
        body.AddForce(-transform.right * Mathf.Clamp(lateral * GripAssist, -3, 3), ForceMode.Acceleration);
        // Pair wheels by axle, independent of the order in the prefab's array.
        for (int i = 0; i < Wheels.Length; i++)
        {
            var left = Wheels[i];
            if (left == null || !left.Collider) continue;
            Vector3 lp = transform.InverseTransformPoint(left.Collider.transform.position);
            if (lp.x >= 0) continue;
            for (int j = 0; j < Wheels.Length; j++)
            {
                var right = Wheels[j];
                if (right == null || !right.Collider) continue;
                Vector3 rp = transform.InverseTransformPoint(right.Collider.transform.position);
                if (rp.x <= 0 || Mathf.Abs(rp.z - lp.z) > 0.3f) continue;
                float lt = SuspensionTravel(left.Collider, out bool lg);
                float rt = SuspensionTravel(right.Collider, out bool rg);
                float force = (lt - rt) * AntiRollStrength;
                if (lg) body.AddForceAtPosition(transform.up * -force, left.Collider.transform.position);
                if (rg) body.AddForceAtPosition(transform.up * force, right.Collider.transform.position);
                break;
            }
        }
    }

    private static float SuspensionTravel(WheelCollider wheel, out bool grounded)
    {
        grounded = wheel.GetGroundHit(out var hit);
        if (!grounded) return 1;
        return Mathf.Clamp01((-wheel.transform.InverseTransformPoint(hit.point).y - wheel.radius) / Mathf.Max(0.01f, wheel.suspensionDistance));
    }

    private void Update()
    {
        if (Wheels == null) return;
        foreach (var w in Wheels)
        {
            if (w == null || !w.Collider || !w.Model) continue;
            w.Collider.GetWorldPose(out var position, out var rotation);
            w.Model.SetPositionAndRotation(position, rotation);
        }
    }

    private void SyncAppearance()
    {
        if (!appearance) return;
        appearance.currentSpeed = SpeedKph;
        appearance.desiredSpeed = TargetSpeedKph;
        appearance.stoppedForVehicle = WaitingForOwnCar;
        appearance.stoppedForTrafficLight = WaitingForLight;
        appearance.throttleInput = throttle;
        appearance.brakeInput = brake;
        appearance.steerInput = SteeringInput;
        if (appearance.lights == null) return;
        bool blink = Time.time % 0.7f < 0.35f;
        foreach (var lamp in appearance.lights)
        {
            if (lamp == null || !lamp.light) continue;
            bool on = lamp.lightType == RTC_CarController.LightType.Brake ? brake > 0.1f :
                lamp.lightType == RTC_CarController.LightType.Headlight ? appearance.isNight :
                blink && (lamp.lightType == RTC_CarController.LightType.Indicator_L ? SteeringInput < -0.15f : SteeringInput > 0.15f);
            lamp.light.intensity = on ? lamp.intensity : 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider.GetComponentInParent<ArcadeTrafficCar>();
        var rtc = other ? null : collision.collider.GetComponentInParent<RTC_CarController>();
        if (!other && !rtc) return;
        int stream = other ? other.StreamId : rtc.CarSpawnIndex;
        if (stream == StreamId || collision.relativeVelocity.magnitude < ForeignCrashImpact) return;
        if (StopAfterForeignCrash) HasCrashed = true;
        var director = GameManager.ForScene(gameObject.scene);
        if (director) director.RecordCollision(gameObject, other ? other.gameObject : rtc.gameObject);
    }

    private void ApplyFailSafeBrakes()
    {
        if (Wheels == null) return;
        foreach (var w in Wheels) if (w != null && w.Collider) { w.Collider.motorTorque = 0; w.Collider.brakeTorque = 10000; }
    }
    private void OnDisable() { ApplyFailSafeBrakes(); }
    private void OnDrawGizmosSelected()
    {
        if (!DrawPath || !initialized) return;
        Gizmos.color = WaitingForLight || WaitingForOwnCar ? Color.red : Color.cyan;
        path.Draw();
        Gizmos.DrawWireSphere(target, 0.2f);
    }
}
