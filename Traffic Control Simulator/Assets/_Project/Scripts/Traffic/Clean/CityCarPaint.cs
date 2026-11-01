using UnityEngine;
namespace TrafficControl.CleanVehicles
{
    public sealed class CityCarPaint : MonoBehaviour
    {
        public Material OwnedMaterial;
        private void OnDestroy() { if (OwnedMaterial) Destroy(OwnedMaterial); }
    }
}
