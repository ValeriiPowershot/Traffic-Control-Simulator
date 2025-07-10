namespace Gley.UrbanAssets.Internal
{
    /// <summary>
    /// Used to set the intersection on waypoint
    /// </summary>
    public interface IIntersection
    {
        bool IsPathFree(int waypointIndex);
        void VehicleLeft(int vehicleIndex);
        void VehicleEnter(int vehicleIndex);
#if GLEY_PEDESTRIAN_SYSTEM
        void PedestrianPassed(int agentIndex);
#endif
    }
}