using System;
using TrafficControl.CleanVehicles;

static class Program
{
    static int count;
    static void Check(bool ok,string text) {if (!ok) throw new Exception(text); count++;}
    static void Main()
    {
        Check(CityCarMath.SafeSpeed(0,10,3,0.45f)==0,"No clearance means stop even if leader moves");
        Check(CityCarMath.SafeSpeed(-1,0,3,0.45f)==0,"Overlap means stop");
        Check(CityCarMath.SafeSpeed(10,0,3,0.45f)>CityCarMath.SafeSpeed(2,0,3,0.45f),"Longer gap permits more speed");
        Check(CityCarMath.SafeSpeed(10,5,3,0.45f)>CityCarMath.SafeSpeed(10,0,3,0.45f),"Moving queue permits more speed");
        Check(CityCarMath.SafeSpeed(10,0,3,1)<CityCarMath.SafeSpeed(10,0,3,0.1f),"Reaction margin lowers approach speed");
        Check(CityCarMath.RoadWheelAngle(0,0,5,0,2.4f,1.8f,0.8f)==0,"Straight centred vehicle");
        Check(CityCarMath.RoadWheelAngle(0,1,5,0,2.4f,1.8f,0.8f)<0,"Right lane deviation steers left");
        Check(CityCarMath.RoadWheelAngle(0,-1,5,0,2.4f,1.8f,0.8f)>0,"Left lane deviation steers right");
        Check(CityCarMath.RoadWheelAngle(0,0,5,0.2f,2.4f,1.8f,0.8f)>0,"Curvature anticipates turn");
        Check(Math.Abs(CityCarMath.RoadWheelAngle(3,10,0,3,2.4f,1.8f,0.8f))<=0.8f,"Wheel angle remains bounded");
        Check(CityCarMath.BendSpeed(0.2f,3)<CityCarMath.BendSpeed(0.05f,3),"Tighter bend lowers speed");
        Console.WriteLine(count+" clean-controller checks passed");
    }
}
