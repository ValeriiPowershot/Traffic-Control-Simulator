using System.Collections.Generic;
using UnityEngine;
namespace Gley.UrbanAssets.Internal
{
    public class WaypointSettingsBase : MonoBehaviour
    {
        public List<WaypointSettingsBase> neighbors;
        public List<WaypointSettingsBase> prev;

        //public bool stop;
        public bool draw = true;
        public bool inView;
        public Vector3 position;

        //priority
        public bool priorityLocked;
        public int priority;

        //events
        public bool triggerEvent;
        public string eventData;

        //path finding
        public List<int> distance;
        public bool penaltyLocked;
        public int penalty;

        public virtual void Initialize()
        {
            neighbors = new List<WaypointSettingsBase>();
            prev = new List<WaypointSettingsBase>();
        }

        public virtual void VerifyAssignments()
        {
            if (neighbors == null)
            {
                neighbors = new List<WaypointSettingsBase>();
            }

            for (int j = neighbors.Count - 1; j >= 0; j--)
            {
                if (neighbors[j] == null)
                {
                    neighbors.RemoveAt(j);
                }
            }

            if (prev == null)
            {
                prev = new List<WaypointSettingsBase>();
            }

            for (int j = prev.Count - 1; j >= 0; j--)
            {
                if (prev[j] == null)
                {
                    prev.RemoveAt(j);
                }
            }

            if (distance == null)
            {
                distance = new List<int>();
            }

            if (priority <= 0)
            {
                priority = 1;
            }
        }
    }
}