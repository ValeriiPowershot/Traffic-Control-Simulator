using System;

public static class ArcadeTrafficRules
{
    public static bool ShouldFollow(int mine, int other) => mine == other;
    public static float ApproachSpeed(float endSpeed, float distance, float deceleration)
        => (float)Math.Sqrt(endSpeed * endSpeed + 2 * Math.Max(0.1f, deceleration) * Math.Max(0, distance));
    public static float Steering(float x, float z, float wheelbase, float maxRadians)
    {
        double d = x * x + z * z;
        if (d < 0.001) return 0;
        return (float)Math.Max(-1, Math.Min(1, Math.Atan2(2 * wheelbase * x, d) / Math.Max(0.01f, maxRadians)));
    }
    public static float FollowSpeed(float leaderSpeed, float gap, float standstillGap, float reactionTime, float deceleration)
    {
        if (gap <= standstillGap) return 0;
        float distance = Math.Max(0, gap - standstillGap);
        float matching = Math.Max(0, leaderSpeed + (distance - leaderSpeed * reactionTime) / Math.Max(0.1f, reactionTime));
        return Math.Min(matching, ApproachSpeed(leaderSpeed, distance, deceleration));
    }
}
