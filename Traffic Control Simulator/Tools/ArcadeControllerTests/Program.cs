using System;

static class Program
{
    static int passed;
    static void Check(bool ok, string description) { if (!ok) throw new Exception(description); passed++; }
    static void Main()
    {
        Check(ArcadeTrafficRules.ShouldFollow(7, 7), "Follow own stream");
        Check(!ArcadeTrafficRules.ShouldFollow(7, 99), "Foreign stream is not a braking obstacle");
        Check(ArcadeTrafficRules.ShouldFollow(0, 0), "Zero is a valid shared stream");
        Check(ArcadeTrafficRules.FollowSpeed(10, 1, 1.1f, 1, 3) == 0, "Full stop inside standstill gap");
        Check(ArcadeTrafficRules.FollowSpeed(0, 2, 1.1f, 1, 3) < 1, "Slow approach to queue");
        Check(ArcadeTrafficRules.FollowSpeed(5, 6.1f, 1.1f, 1, 3) == 5, "Match leader at headway");
        Check(ArcadeTrafficRules.ApproachSpeed(0, 0, 3) == 0, "Stop at red line");
        Check(Math.Abs(ArcadeTrafficRules.ApproachSpeed(0, 6, 3) - 6) < 0.0001, "Physical braking envelope");
        Check(ArcadeTrafficRules.Steering(0, 4, 2.4f, 0.8f) == 0, "Straight road");
        Check(ArcadeTrafficRules.Steering(2, 4, 2.4f, 0.8f) > 0, "Right steering sign");
        Check(ArcadeTrafficRules.Steering(-2, 4, 2.4f, 0.8f) < 0, "Left steering sign");
        Check(Math.Abs(ArcadeTrafficRules.Steering(1, 0, 10, 0.8f)) <= 1, "Wheel steering is bounded");
        Check(ArcadeTrafficRules.Steering(0, 0, 2.4f, 0.8f) == 0, "Coincident target cannot produce NaN");
        Console.WriteLine(passed + " own-controller checks passed");
    }
}
