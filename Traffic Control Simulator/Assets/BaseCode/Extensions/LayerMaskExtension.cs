using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Extensions
{
    public static class LayerMaskExtension
    {
        public static bool IsSameLayer(this LayerMask layerMask, Collider other)
        {
            return layerMask.value == (1 << other.gameObject.layer);
        }
        
    }
    
}