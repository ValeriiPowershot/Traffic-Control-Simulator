namespace Gley.TrafficSystem.Internal
{
    public interface IVehicleLightsComponent
    {
        void DeactivateLights();
        void Initialize();
        void SetBlinker(BlinkType blinkLeft);
        void SetBrakeLights(bool brake);
        void SetMainLights(bool mainLights);
        void SetReverseLights(bool reverse);
        void UpdateLights(float realtimeSinceStartup);
    }
}
