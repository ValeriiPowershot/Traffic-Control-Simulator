using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Saves layer settings
    /// </summary>
    public class LayerSetup : ScriptableObject
    {
        public bool edited;
        public LayerMask roadLayers = 256;
        public LayerMask trafficLayers = 512;
        public LayerMask buildingsLayers = 1024;
        public LayerMask obstaclesLayers = 2048;
        public LayerMask playerLayers = 4096;
    }
}
