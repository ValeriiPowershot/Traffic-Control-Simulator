namespace Gley.TrafficSystem
{
    public enum DriveActions
    {
        Forward = 0,
        ForceForward = 1, // this will clear all the existing actions and go forward 
        Continue = 10, //do not change current state
        ChangeLane = 11,
        Follow = 12, //
        Overtake = 13,
        AvoidForward = 15, // mirror wheel rotation, keep direction,
        GiveWay = 16,
        StopInPoint = 20, //traffic light, other road signs
        StopInDistance = 70, // stop in front trigger dimension
        StopTemp = 90, //stop instantly
        Stop = 91,
        Reverse = 80, //keep wheels as they are
        AvoidReverse = 100, //change wheel direction
        NoWaypoint = 1000,
        NoPath = 1001,
        Destroyed = 10000
    }
}