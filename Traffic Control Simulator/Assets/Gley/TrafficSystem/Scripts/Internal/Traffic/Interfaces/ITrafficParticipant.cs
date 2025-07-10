namespace Gley.TrafficSystem.Internal
{
    public interface ITrafficParticipant
    {
        //returns the rb.velocity
        public float GetCurrentSpeedMS();
    }
}