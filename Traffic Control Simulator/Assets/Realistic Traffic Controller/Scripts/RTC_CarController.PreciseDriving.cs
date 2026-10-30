using UnityEngine;

namespace Realistic_Traffic_Controller.Scripts
{
    public partial class RTC_CarController
    {
        [Header("Precise city driving")]
        [Tooltip("Keep WheelCollider physics; use one fixed-step path and speed controller.")]
        public bool preciseDriving = true;
        [Min(1f)] public float citySpeedLimit = 35f;
        [Min(0.2f)] public float steeringLookAhead = 1.6f;
        [Min(0f)] public float steeringPreviewTime = 0.25f;
        [Min(0.2f)] public float maximumLookAhead = 4f;
        [Min(0.5f)] public float comfortableDeceleration = 3.5f;
        [Min(0.5f)] public float lateralAccelerationLimit = 2.8f;
        [Min(0.1f)] public float standstillGap = 1.2f;
        [Min(0.1f)] public float followingTime = 1.1f;
        [Min(1f)] public float steeringResponse = 6f;
        [Range(20f, 60f)] public float citySteeringAngle = 48f;

        private readonly RaycastHit[] drivingHits = new RaycastHit[64];
        private readonly Collider[] overlappingVehicles = new Collider[32];
        private readonly Vector3[] drivingPath = new Vector3[16];
        private Vector3 drivingStart;
        private float drivingWheelbase;
        private float rearAxleZ;
        private TrafficLight observedLight;
        private Collider observedLightCollider;

        private void ResetPreciseDriving()
        {
            drivingStart = transform.position;
            observedLight = null;
            observedLightCollider = null;
            drivingWheelbase = 0f;
            startBoostActive = false;
            stoppedForTrafficLight = stoppedForVehicle = stoppedForPoint = false;
            if (preciseDriving) steerAngle = citySteeringAngle;
        }

        private void MeasureAxles()
        {
            float front = float.MinValue, rear = float.MaxValue;
            if (wheels != null)
                foreach (var wheel in wheels)
                {
                    if (wheel == null || wheel.wheelCollider == null) continue;
                    float z = transform.InverseTransformPoint(wheel.wheelCollider.transform.position).z;
                    if (wheel.isSteering) front = Mathf.Max(front, z);
                    else rear = Mathf.Min(rear, z);
                }
            drivingWheelbase = front > rear ? front - rear : Mathf.Max(1f, (bounds.front - bounds.rear) * 0.6f);
            rearAxleZ = rear < float.MaxValue ? rear : -drivingWheelbase * 0.5f;
        }

        private static Vector3 Flat(Vector3 value) { value.y = 0; return value; }

        private float CornerTrim(Vector3 incoming, Vector3 outgoing)
        {
            float angle = Vector3.Angle(incoming, outgoing) * Mathf.Deg2Rad;
            if (angle < 0.05f || angle > 2.8f) return 0;
            float radius = drivingWheelbase / Mathf.Tan(Mathf.Max(1, steerAngle) * Mathf.Deg2Rad);
            return Mathf.Min(radius * Mathf.Tan(angle * 0.5f) * 1.6f, incoming.magnitude * 0.35f, outgoing.magnitude * 0.35f);
        }

        private Vector3 PathTarget(Vector3 start, Vector3 corner, Vector3 end, Vector3 axle, float preview, float trim)
        {
            // Round only the already committed turn. Never choose a branch while looking ahead.
            Vector3 entry = corner - Flat(corner - start).normalized * trim;
            Vector3 exit = corner + Flat(end - corner).normalized * trim;
            drivingPath[0] = start;
            drivingPath[1] = entry;
            for (int i = 1; i <= 12; i++)
            {
                float t = i / 12f;
                drivingPath[i + 1] = (1 - t) * (1 - t) * entry + 2 * (1 - t) * t * corner + t * t * exit;
            }
            drivingPath[14] = end;
            float nearest = float.PositiveInfinity;
            int segmentIndex = 0;
            Vector3 projectionPoint = start;
            for (int i = 0; i < 14; i++)
            {
                Vector3 line = Flat(drivingPath[i + 1] - drivingPath[i]);
                float t = line.sqrMagnitude > 0.0001f ? Mathf.Clamp01(Vector3.Dot(Flat(axle - drivingPath[i]), line) / line.sqrMagnitude) : 0;
                Vector3 point = drivingPath[i] + line * t;
                float distance = Flat(point - axle).sqrMagnitude;
                if (distance < nearest) { nearest = distance; segmentIndex = i; projectionPoint = point; }
            }
            for (int i = segmentIndex; i < 14; i++)
            {
                Vector3 line = Flat(drivingPath[i + 1] - projectionPoint);
                if (line.magnitude >= preview) return projectionPoint + line.normalized * preview;
                preview -= line.magnitude;
                projectionPoint = drivingPath[i + 1];
            }
            return end;
        }

        private void PreciseDrivingStep()
        {
            if (drivingWheelbase <= 0) MeasureAxles();
            float dt = Time.fixedDeltaTime;
            float speed = Flat(Rigid.velocity).magnitude;
            Vector3 axle = transform.TransformPoint(new Vector3(0, 0, rearAxleZ));
            startBoostActive = false;

            if (waitingAtWaypoint > 0) waitingAtWaypoint = Mathf.Max(0, waitingAtWaypoint - dt);
            if (!crashed && currentWaypoint && waitingAtWaypoint <= 0)
            {
                // Advance using the physical rear axle, never the look-ahead target.
                for (int i = 0; i < 4 && currentWaypoint; i++)
                {
                    Vector3 end = currentWaypoint.transform.position;
                    Vector3 start = pastWaypoint && pastWaypoint != currentWaypoint ? pastWaypoint.transform.position : drivingStart;
                    Vector3 segment = Flat(end - start);
                    Vector3 offset = Flat(axle - end);
                    bool crossed = segment.sqrMagnitude > 0.01f && Vector3.Dot(offset, segment.normalized) >= 0;
                    bool close = offset.magnitude < Mathf.Min(0.7f, currentWaypoint.radius);
                    if (nextWaypoint && nextWaypoint != currentWaypoint)
                    {
                        Vector3 outgoing = Flat(nextWaypoint.transform.position - end);
                        float trim = CornerTrim(segment, outgoing);
                        if (trim > 0.05f)
                        {
                            crossed = Vector3.Dot(Flat(axle - end) - outgoing.normalized * trim, outgoing.normalized) >= 0;
                            close = false;
                        }
                    }
                    if (!crossed && !close) break;
                    PassWaypoint();
                    if (waitingAtWaypoint > 0) break;
                }
            }

            float targetSpeed = 0;
            if (currentWaypoint && !crashed)
            {
                Vector3 end = currentWaypoint.transform.position;
                Vector3 start = pastWaypoint && pastWaypoint != currentWaypoint ? pastWaypoint.transform.position : drivingStart;
                Vector3 segment = Flat(end - start);
                float length = segment.magnitude;
                Vector3 tangent = length > 0.01f ? segment / length : Flat(transform.forward).normalized;
                float progress = Mathf.Clamp(Vector3.Dot(Flat(axle - start), tangent), 0, length);
                float remaining = Mathf.Max(0, length - progress);
                float preview = Mathf.Clamp(steeringLookAhead + speed * steeringPreviewTime, 0.3f, Mathf.Max(0.3f, maximumLookAhead));
                Vector3 target = start + tangent * Mathf.Min(length, progress + preview);
                float turnSpeed = float.PositiveInfinity;

                if (nextWaypoint && nextWaypoint != currentWaypoint)
                {
                    Vector3 outgoing = Flat(nextWaypoint.transform.position - end);
                    float angle = Vector3.Angle(tangent, outgoing) * Mathf.Deg2Rad;
                    // Limit corner cutting: only a short distance into the committed outgoing segment.
                    float beyond = Mathf.Min(Mathf.Max(0, preview - remaining), Mathf.Min(outgoing.magnitude * 0.3f, 0.65f));
                    if (preview > remaining) target = end + outgoing.normalized * beyond;
                    float bendLength = Mathf.Max(1f, Mathf.Min(length, outgoing.magnitude));
                    float curvature = 2f * Mathf.Sin(angle * 0.5f) / bendLength;
                    float trim = CornerTrim(segment, outgoing);
                    if (trim > 0.05f)
                    {
                        target = PathTarget(start, end, nextWaypoint.transform.position, axle, Mathf.Min(preview, 2.5f), trim);
                        // Maximum curvature of the quadratic at its midpoint.
                        curvature = Mathf.Max(curvature, Mathf.Sin(angle * 0.5f) / Mathf.Max(0.1f, trim * Mathf.Pow(Mathf.Cos(angle * 0.5f), 2)));
                        remaining = Mathf.Max(0, remaining - trim);
                    }
                    turnSpeed = TrafficDrivingMath.CurveSpeed(curvature, lateralAccelerationLimit);
                }

                Vector3 localTarget = transform.InverseTransformDirection(Flat(target - axle));
                steerInputRaw = TrafficDrivingMath.Steering(localTarget.x, localTarget.z, drivingWheelbase, steerAngle * Mathf.Deg2Rad);
                // Recover a target behind the axle with a full, physically steered turn.
                if (localTarget.z < -0.1f) steerInputRaw = localTarget.x >= 0 ? 1 : -1;
                float laneSpeed = interconnecting ? currentWaypoint.desiredSpeedForInterConnectionWaypoint : currentWaypoint.desiredSpeedForNextWaypoint;
                targetSpeed = Mathf.Min(Mathf.Max(0, laneSpeed), maximumSpeed, Mathf.Max(1, citySpeedLimit)) / 3.6f;
                targetSpeed = Mathf.Min(targetSpeed, TrafficDrivingMath.ApproachSpeed(turnSpeed, Mathf.Max(0, remaining - drivingWheelbase), comfortableDeceleration));
                float steeringCurvature = Mathf.Abs(Mathf.Tan(steerInputRaw * steerAngle * Mathf.Deg2Rad)) / drivingWheelbase;
                targetSpeed = Mathf.Min(targetSpeed, TrafficDrivingMath.CurveSpeed(steeringCurvature, lateralAccelerationLimit));
                if (!nextWaypoint)
                    targetSpeed = Mathf.Min(targetSpeed, TrafficDrivingMath.ApproachSpeed(0, remaining, comfortableDeceleration));
                navigatorPoint.position = target;
                AimNavigator();
                targetSpeed = SenseDrivingObstacles(targetSpeed, speed, target);
                willTurnLeft = steerInputRaw < -0.15f;
                willTurnRight = steerInputRaw > 0.15f;
            }
            else steerInputRaw = 0;

            if (crashed || !currentWaypoint || waitingAtWaypoint > 0 || stopNow) targetSpeed = 0;
            desiredSpeed = targetSpeed * 3.6f;
            float error = targetSpeed - speed;
            brakeInputRaw = targetSpeed < 0.05f ? 1 : Mathf.Clamp01(-error / 2f);
            throttleInputRaw = brakeInputRaw > 0.01f ? 0 : Mathf.Clamp01(error / 2f);
            if (!engineRunning) throttleInputRaw = 0;
            if (!ignoreInputs)
            {
                steerInput = Mathf.MoveTowards(steerInput, steerInputRaw, steeringResponse * dt);
                brakeInput = Mathf.MoveTowards(brakeInput, brakeInputRaw, 6f * dt);
                throttleInput = brakeInput > 0.01f ? 0 : Mathf.MoveTowards(throttleInput, throttleInputRaw, 1.8f * dt);
            }
            ClampInputs();
            stoppedForReason = brakeInput > 0.05f;
            stoppedForPoint = targetSpeed < 0.05f && !stoppedForVehicle && !stoppedForTrafficLight;
            UpdateProjection();
        }

        private float SenseDrivingObstacles(float targetSpeed, float speed, Vector3 target)
        {
            stoppedForVehicle = stoppedForTrafficLight = false;
            raycastedVehicle = null;
            raycastHitDistance = 0;
            Vector3 origin = transform.TransformPoint(new Vector3(0, Mathf.Lerp(bounds.down, bounds.up, 0.55f), bounds.front));
            float range = Mathf.Max(8, speed * followingTime + speed * speed / (2 * Mathf.Max(0.5f, comfortableDeceleration)) + standstillGap);
            raycastDistance = range;
            Vector3 directionToPath = Flat(target - origin);
            if (directionToPath.sqrMagnitude < 0.01f || Vector3.Dot(directionToPath, transform.forward) <= 0) directionToPath = transform.forward;
            ScanDrivingCorridor(origin, Flat(transform.forward).normalized, range, speed, ref targetSpeed);
            ScanDrivingCorridor(origin, directionToPath.normalized, range, speed, ref targetSpeed);
            // Casts do not report colliders already overlapping the cast origin.
            int overlaps = Physics.OverlapSphereNonAlloc(origin, Mathf.Max(0.15f, Mathf.Abs(bounds.right) * 0.65f), overlappingVehicles, raycastLayermask, QueryTriggerInteraction.Ignore);
            if (overlaps == overlappingVehicles.Length) targetSpeed = 0;
            for (int i = 0; i < overlaps; i++)
            {
                var other = overlappingVehicles[i].GetComponentInParent<RTC_CarController>();
                if (other && other != this)
                {
                    targetSpeed = 0;
                    stoppedForVehicle = true;
                    raycastedVehicle = other;
                    raycastHitDistance = 0;
                }
            }

            // Keep a red light remembered while its collider is occluded by the queue.
            if (observedLight && observedLightCollider && observedLightCollider.enabled && observedLightCollider.gameObject.activeInHierarchy && observedLight.LightState != TrafficLightState.Green)
            {
                Vector3 closest = observedLightCollider.ClosestPoint(origin);
                float forward = Vector3.Dot(Flat(closest - origin), Flat(transform.forward).normalized);
                if (forward >= -0.25f)
                {
                    stoppedForTrafficLight = true;
                    float distance = Mathf.Max(0, Flat(closest - origin).magnitude - standstillGap);
                    targetSpeed = Mathf.Min(targetSpeed, TrafficDrivingMath.ApproachSpeed(0, distance, comfortableDeceleration));
                }
                else { observedLight = null; observedLightCollider = null; }
            }
            else { observedLight = null; observedLightCollider = null; }
            return targetSpeed;
        }

        private void ScanDrivingCorridor(Vector3 origin, Vector3 direction, float range, float speed, ref float targetSpeed)
        {
            float width = Mathf.Max(0.15f, Mathf.Min(Mathf.Abs(bounds.left), Mathf.Abs(bounds.right)) * 0.65f);
            int count = Physics.SphereCastNonAlloc(origin, width, direction, drivingHits, range, raycastLayermask, QueryTriggerInteraction.Collide);
            // A saturated query is incomplete: stop instead of assuming a clear road.
            if (count == drivingHits.Length) targetSpeed = 0;
            for (int i = 0; i < count; i++)
            {
                var hit = drivingHits[i];
                if (!hit.collider || hit.transform.IsChildOf(transform)) continue;
                var light = hit.collider.GetComponentInParent<TrafficLight>();
                if (!light && hit.collider.gameObject.layer == LayerMask.NameToLayer("RTC_TrafficLight"))
                    light = hit.transform.root.GetComponentInChildren<TrafficLight>();
                if (light)
                {
                    if (light.TrafficLightSpawnIndex == CarSpawnIndex && light.LightState != TrafficLightState.Green)
                    {
                        observedLight = light;
                        observedLightCollider = hit.collider;
                        stoppedForTrafficLight = true;
                        targetSpeed = Mathf.Min(targetSpeed, TrafficDrivingMath.ApproachSpeed(0, Mathf.Max(0, hit.distance - standstillGap), comfortableDeceleration));
                    }
                    continue;
                }
                var other = hit.collider.GetComponentInParent<RTC_CarController>();
                if (!other || other == this || hit.collider.isTrigger) continue;
                float leaderSpeed = Mathf.Max(0, Vector3.Dot(Flat(other.Rigid.velocity), direction));
                float gap = Mathf.Max(0, hit.distance);
                float followingSpeed = TrafficDrivingMath.FollowingSpeed(leaderSpeed, gap, standstillGap, followingTime);
                float stoppingSpeed = TrafficDrivingMath.ApproachSpeed(leaderSpeed, Mathf.Max(0, gap - standstillGap), comfortableDeceleration);
                targetSpeed = Mathf.Min(targetSpeed, followingSpeed, stoppingSpeed);
                stoppedForVehicle = true;
                if (!raycastedVehicle || gap < raycastHitDistance)
                {
                    raycastedVehicle = other;
                    raycastHitDistance = gap;
                }
            }
        }
    }
}
