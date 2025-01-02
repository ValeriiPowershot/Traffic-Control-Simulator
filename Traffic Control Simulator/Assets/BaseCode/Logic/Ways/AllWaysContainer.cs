using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Ways
{
    public class AllWaysContainer : MonoBehaviour
    {
        [FormerlySerializedAs("AllWays")] 
        public WaypointContainer[] allWays;
    }
    
    
}
