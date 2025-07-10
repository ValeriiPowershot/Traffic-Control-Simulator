using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class RoadCreator<T, R, X> : Creator where T : RoadBase where R : GenericConnectionPool<X> where X : ConnectionCurveBase
    {
        private Transform waypointsHolder;
        protected RoadData<T> data;

        protected void Initialize(RoadData<T> data)
        {
            this.data = data;
        }


        internal void DeleteCurrentRoad(RoadBase road)
        {
            Undo.DestroyObjectImmediate(road.gameObject);
            data.TriggerModifiedEvent();
        }


        protected Transform GetRoadWaypointsHolder(string trafficWaypointsHolderName)
        {
            bool editingInsidePrefab = GleyPrefabUtilities.EditingInsidePrefab();

            if (waypointsHolder == null || waypointsHolder.name != trafficWaypointsHolderName)
            {
                GameObject holder = null;
                if (editingInsidePrefab)
                {
                    GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                    Transform waypointsHolder = prefabRoot.transform.Find(trafficWaypointsHolderName);
                    if (waypointsHolder == null)
                    {
                        waypointsHolder = new GameObject(trafficWaypointsHolderName).transform;
                        waypointsHolder.SetParent(prefabRoot.transform);
                        waypointsHolder.gameObject.AddComponent<R>();
                        waypointsHolder.gameObject.tag = Constants.editorTag;
                    }
                    holder = waypointsHolder.gameObject;
                }
                else
                {
                    GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(obj => obj.name == trafficWaypointsHolderName).ToArray();
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
                    if (holder == null)
                    {
                        holder = new GameObject(trafficWaypointsHolderName);
                        holder.AddComponent<R>();
                    }
                }
                waypointsHolder = holder.transform;
            }
            return waypointsHolder;
        }


        protected int GetFreeRoadNumber(string trafficWaypointsHolderName)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < GetRoadWaypointsHolder(trafficWaypointsHolderName).childCount; i++)
            {
                int.TryParse(waypointsHolder.GetChild(i).name.Split('_')[1], out var number);
                numbers.Add(number);
            }
            return FindSmallestMissingNumber(numbers);
        }

        private int FindSmallestMissingNumber(List<int> numbers)
        {
            numbers.Sort();

            if (numbers.Count == 0 || numbers[0] > 1)
            {
                return 1;
            }

            for (int i = 0; i < numbers.Count - 1; i++)
            {
                if (numbers[i + 1] - numbers[i] > 1)
                {
                    return numbers[i] + 1;
                }
            }

            return numbers[numbers.Count - 1] + 1;
        }
    }
}
