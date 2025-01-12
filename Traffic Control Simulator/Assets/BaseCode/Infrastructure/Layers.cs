using UnityEngine;

namespace BaseCode.Infrastructure
{
    public static class Layers
    {
        public const int Car = 7;
        public const int StopLine  = 10;
        
        public static readonly int AlternativeWay = LayerMask.NameToLayer("Car");  // why not
    }
}
