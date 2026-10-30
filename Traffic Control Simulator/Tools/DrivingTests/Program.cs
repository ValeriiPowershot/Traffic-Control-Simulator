using System;
using Realistic_Traffic_Controller.Scripts;

static class Program
{
    static int checks;
    static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        checks++;
    }
    static void Main()
    {
        float angle = (float)Math.PI / 4;
        Check(TrafficDrivingMath.Steering(0, 5, 2.5f, angle) == 0, "Straight road should not steer");
        Check(TrafficDrivingMath.Steering(2, 5, 2.5f, angle) > 0, "Right turn");
        Check(TrafficDrivingMath.Steering(-2, 5, 2.5f, angle) < 0, "Left turn");
        Check(Math.Abs(TrafficDrivingMath.Steering(-2, 5, 2.5f, angle) + TrafficDrivingMath.Steering(2, 5, 2.5f, angle)) < 0.0001, "Symmetric steering");
        Check(TrafficDrivingMath.Steering(0, 0, 2.5f, angle) == 0, "Coincident target is finite");
        Check(TrafficDrivingMath.Steering(1, 0, 8, angle) == 1, "Physical steering limit");
        Check(TrafficDrivingMath.CurveSpeed(0.5f, 3) < TrafficDrivingMath.CurveSpeed(0.1f, 3), "Tighter turns need lower speed");
        Check(TrafficDrivingMath.ApproachSpeed(0, 0, 3.5f) == 0, "Stop at stop line");
        Check(Math.Abs(TrafficDrivingMath.ApproachSpeed(0, 10, 3.5f) - Math.Sqrt(70)) < 0.0001, "Braking-distance envelope");
        Check(TrafficDrivingMath.ApproachSpeed(4, 0, 3.5f) == 4, "Reach bend at its target speed");
        Check(TrafficDrivingMath.FollowingSpeed(10, 1, 1.2f, 1.1f) == 0, "Do not accelerate into close leader");
        Check(Math.Abs(TrafficDrivingMath.FollowingSpeed(5, 6.7f, 1.2f, 1.1f) - 5) < 0.0001, "Match leader at desired headway");
        Check(TrafficDrivingMath.FollowingSpeed(0, 5, 1.2f, 1.1f) < TrafficDrivingMath.FollowingSpeed(0, 10, 1.2f, 1.1f), "Approach queue progressively");
        // Integrate a kinematic bicycle on a constant-radius bend. This checks sign,
        // wheelbase and units together, independently of the steering equation.
        double x = 0, z = 0, heading = 0, maxError = 0;
        const double radius = 6, wheelbase = 2.5, velocity = 4, dt = 0.02;
        for (int i = 0; i < 1000; i++)
        {
            double theta = Math.Atan2(z, radius - x) + 0.3;
            double tx = radius - radius * Math.Cos(theta), tz = radius * Math.Sin(theta);
            double dx = tx - x, dz = tz - z;
            float lx = (float)(Math.Cos(heading) * dx - Math.Sin(heading) * dz);
            float lz = (float)(Math.Sin(heading) * dx + Math.Cos(heading) * dz);
            double input = TrafficDrivingMath.Steering(lx, lz, (float)wheelbase, angle);
            heading += velocity / wheelbase * Math.Tan(input * angle) * dt;
            x += Math.Sin(heading) * velocity * dt;
            z += Math.Cos(heading) * velocity * dt;
            maxError = Math.Max(maxError, Math.Abs(Math.Sqrt((x - radius) * (x - radius) + z * z) - radius));
        }
        Check(maxError < 0.15, "Bicycle must track 6m bend within 15cm; error=" + maxError);
        Console.WriteLine("PASS: " + checks + " checks; bicycle radial error " + maxError.ToString("F3") + "m. WheelCollider/scene testing is separate.");
    }
}
