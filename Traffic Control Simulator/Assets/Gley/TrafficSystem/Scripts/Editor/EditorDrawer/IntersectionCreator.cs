using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class IntersectionCreator : Creator
    {
        const string intersectionPrefix = "Intersection_";
        private Transform intersectionHolder;
        private IntersectionData intersectionData;

        internal IntersectionCreator Initialize(IntersectionData intersectionData)
        {
            this.intersectionData = intersectionData;
            return this;
        }


        internal T Create<T>() where T : GenericIntersectionSettings
        {
            Transform intersection = CreateIntersectionObject();
            return (T)intersection.gameObject.AddComponent<T>().Initialize();
        }


        internal void DeleteIntersection(GenericIntersectionSettings intersection)
        {
            DestroyImmediate(intersection.gameObject);
            intersectionData.TriggerModifiedEvent();
        }


        private Transform CreateIntersectionObject()
        {
            GameObject intersection = new GameObject(intersectionPrefix + GetFreeIntersectionNumber());
            intersection.transform.SetParent(GetIntersectionHolder());
            intersection.gameObject.tag = UrbanAssets.Internal.Constants.editorTag;
            Vector3 poz = SceneView.lastActiveSceneView.camera.transform.position;
            poz.y = 0;
            intersection.transform.position = poz;
            return intersection.transform;
        }


        private int GetFreeIntersectionNumber()
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < GetIntersectionHolder().childCount; i++)
            {
                int.TryParse(GetIntersectionHolder().GetChild(i).name.Split('_')[1], out var number);
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

        private Transform GetIntersectionHolder()
        {
            bool editingInsidePrefab = GleyPrefabUtilities.EditingInsidePrefab();
            if (intersectionHolder == null)
            {
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
                    if (holder == null)
                    {
                        holder = new GameObject(Internal.Constants.intersectionHolderName);
                    }
                }
                intersectionHolder = holder.transform;
            }
            return intersectionHolder;
        }
    }
}