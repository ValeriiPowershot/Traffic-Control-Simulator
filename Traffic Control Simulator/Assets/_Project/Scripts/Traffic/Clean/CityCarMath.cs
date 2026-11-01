using System;

namespace TrafficControl.CleanVehicles
{
    public static class CityCarMath
    {
        // Braking distance plus reaction distance, solved for the follower's speed.
        public static float SafeSpeed(float clearance, float leaderSpeed, float deceleration, float reactionTime)
        {
            if (clearance <= 0) return 0;
            double a = Math.Max(0.1, deceleration), t = Math.Max(0, reactionTime);
            return (float)Math.Max(0, Math.Sqrt(a * a * t * t + leaderSpeed * leaderSpeed + 2 * a * clearance) - a * t);
        }

        // Stanley lateral feedback plus a curvature feed-forward term. Radians throughout.
        public static float RoadWheelAngle(float headingError, float lateralError, float speed, float curvature, float wheelbase, float gain, float limit)
        {
            double angle = headingError - Math.Atan2(gain * lateralError, Math.Max(0, speed) + 2) + Math.Atan(wheelbase * curvature);
            return (float)Math.Max(-limit, Math.Min(limit, angle));
        }
        public static float BendSpeed(float curvature, float lateralAcceleration)
            => (float)Math.Sqrt(Math.Max(0.1, lateralAcceleration) / Math.Max(0.0001, Math.Abs(curvature)));
    }
}
