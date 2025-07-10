using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class IntersectionData : Data
    {
        private GenericIntersectionSettings[] allIntersections;
        private PriorityIntersectionSettings[] allPriorityIntersections;
        private PriorityCrossingSettings[] allPriorityCrossings;
        private TrafficLightsIntersectionSettings[] allTrafficLightsIntersections;
        private TrafficLightsCrossingSettings[] allTrafficLightsCrossings;

        internal new IntersectionData Initialize()
        {
            base.Initialize();
            return this;
        }


        internal GenericIntersectionSettings[] GetAllIntersections()
        {
            return allIntersections;
        }


        internal PriorityIntersectionSettings[] GetPriorityIntersections()
        {
            return allPriorityIntersections;
        }


        internal PriorityCrossingSettings[] GetPriorityCrossings()
        {
            return allPriorityCrossings;
        }


        internal TrafficLightsIntersectionSettings[] GetTrafficLightsIntersections()
        {
            return allTrafficLightsIntersections;
        }


        internal TrafficLightsCrossingSettings[] GetTrafficLightsCrossings()
        {
            return allTrafficLightsCrossings;
        }


        protected override void LoadAllData()
        {
            var allIntersections = new List<GenericIntersectionSettings>();
            var allPriorityIntersections = new List<PriorityIntersectionSettings>();
            var allTrafficLightsIntersections = new List<TrafficLightsIntersectionSettings>();
            var allTrafficLightsCrossings = new List<TrafficLightsCrossingSettings>();
            var allPriorityCrossings = new List<PriorityCrossingSettings>();

            Transform intersectionHolder = GetIntersectionHolder();
            if (intersectionHolder != null)
            {
                for (int i = 0; i < intersectionHolder.childCount; i++)
                {
                    GenericIntersectionSettings intersection = intersectionHolder.GetChild(i).GetComponent<GenericIntersectionSettings>();
                    intersection.position = intersection.transform.position;
                    if (intersection != null)
                    {
                        intersection.VerifyAssignments();
                        allIntersections.Add(intersection);
                        if (intersection.GetType().Equals(typeof(PriorityIntersectionSettings)))
                        {
                            allPriorityIntersections.Add(intersection as PriorityIntersectionSettings);
                        }

                        if (intersection.GetType().Equals(typeof(TrafficLightsIntersectionSettings)))
                        {
                            allTrafficLightsIntersections.Add(intersection as TrafficLightsIntersectionSettings);
                        }

                        if (intersection.GetType().Equals(typeof(TrafficLightsCrossingSettings)))
                        {
                            allTrafficLightsCrossings.Add(intersection as TrafficLightsCrossingSettings);
                        }

                        if (intersection.GetType().Equals(typeof(PriorityCrossingSettings)))
                        {
                            allPriorityCrossings.Add(intersection as PriorityCrossingSettings);
                        }
                    }
                }
            }
            this.allIntersections = allIntersections.ToArray();
            this.allPriorityCrossings = allPriorityCrossings.ToArray();
            this.allPriorityIntersections = allPriorityIntersections.ToArray();
            this.allTrafficLightsCrossings = allTrafficLightsCrossings.ToArray();
            this.allTrafficLightsIntersections = allTrafficLightsIntersections.ToArray();
        }


        private Transform GetIntersectionHolder()
        {
            bool editingInsidePrefab = GleyPrefabUtilities.EditingInsidePrefab();
            GameObject holder = null;
            if (editingInsidePrefab)
            {
                GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                Transform waypointsHolder = prefabRoot.transform.Find(Internal.Constants.intersectionHolderName);
                if (waypointsHolder == null)
                {
                    waypointsHolder = new GameObject(Internal.Constants.intersectionHolderName).transform;
                    waypointsHolder.SetParent(prefabRoot.transform);
                }
                holder = waypointsHolder.gameObject;
            }
            else
            {

                GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(obj => obj.name == Internal.Constants.intersectionHolderName).ToArray();
                if (allObjects.Length > 0)
                {
                    for (int i = 0; i < allObjects.Length; i++)
                    {
                        if (!GleyPrefabUtilities.IsInsidePrefab(allObjects[i]))
                        {
                            holder = allObjects[i];
                            break;
                        }
                    }
                }
            }
            if (holder != null)
            {
                return holder.transform;
            }
            return null;
        }
    }
}