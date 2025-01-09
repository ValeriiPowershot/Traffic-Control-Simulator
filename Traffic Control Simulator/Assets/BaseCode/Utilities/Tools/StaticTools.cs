using UnityEngine;

namespace BaseCode.Utilities.Tools
{
    public class StaticTools
    {
        public static float SqrDistance(Vector3 destination, Vector3 start)
        {
            return (destination - start).sqrMagnitude;
        }
    }
}
