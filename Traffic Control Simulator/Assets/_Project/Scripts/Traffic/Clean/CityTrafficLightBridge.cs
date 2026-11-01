using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    // Only this integration layer knows about the project's pre-existing traffic lights.
    public sealed class CityTrafficLightBridge : MonoBehaviour
    {
        public TrafficLight Source;
        public CityStopLine StopLine;
        public void Connect(TrafficLight source, CityStopLine line)
        { Source = source; StopLine = line; line.Bind(() => Source != null && Source.BlocksTraffic); }
        private void Start() { if (Source && StopLine) Connect(Source, StopLine); }
        private void OnMouseDown() { if (Source) Source.SwitchRedGreen(); }
    }
}
