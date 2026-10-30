using System;

namespace Realistic_Traffic_Controller.Scripts
{
    // Speeds in metres/second, lengths in metres, angles in radians.
    public static class TrafficDrivingMath
    {
        public static float Steering(float localX, float localZ, float wheelbase, float maxAngle)
        {
            double distanceSquared = localX * localX + localZ * localZ;
            if (distanceSquared < 0.001) return 0;
            double angle = Math.Atan2(2 * wheelbase * localX, distanceSquared);
            return (float)Math.Max(-1, Math.Min(1, angle / Math.Max(0.01, maxAngle)));
        }

        public static float ApproachSpeed(float exitSpeed, float distance, float deceleration)
        {
            return (float)Math.Sqrt(exitSpeed * exitSpeed + 2 * Math.Max(0.1f, deceleration) * Math.Max(0, distance));
        }

        public static float CurveSpeed(float curvature, float lateralAcceleration)
        {
            return (float)Math.Sqrt(Math.Max(0.1f, lateralAcceleration) / Math.Max(0.0001f, Math.Abs(curvature)));
        }

        public static float FollowingSpeed(float leaderSpeed, float gap, float standstillGap, float headway)
        {
            if (gap <= standstillGap) return 0;
            return Math.Max(0, leaderSpeed + (gap - standstillGap - leaderSpeed * headway) / Math.Max(0.1f, headway));
        }
    }
}
