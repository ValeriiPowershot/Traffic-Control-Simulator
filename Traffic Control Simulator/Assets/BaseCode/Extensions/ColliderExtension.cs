using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Extensions
{
    public static class ColliderExtension
    {
        public static bool TryGetSameLayerComponent<T>(this Collider other, LayerMask layerMask, out T component) where T : Component
        {
            if (layerMask.IsSameLayer(other) && other.TryGetComponent(out component))
            {
                return true;
            }

            component = null;
            return false;
        }
    }
    
}