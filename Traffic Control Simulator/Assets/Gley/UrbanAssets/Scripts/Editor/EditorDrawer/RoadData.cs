using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class RoadData<T> : Data where T : RoadBase
    {
        protected T[] allRoads;
        private bool hasErrors;

        public abstract T[] GetAllRoads();

        internal bool HasErrors()
        {
            return hasErrors;
        }

        protected override void LoadAllData()
        {
            List<T> tempRoads;
            if (GleyPrefabUtilities.EditingInsidePrefab())
            {
                GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                tempRoads = prefabRoot.GetComponentsInChildren<T>().ToList();
                for (int i = 0; i < tempRoads.Count; i++)
                {
                    tempRoads[i].positionOffset = prefabRoot.transform.position;
                    tempRoads[i].rotationOffset = prefabRoot.transform.localEulerAngles;
                }
            }
            else
            {
                tempRoads = FindObjectsByType<T>(FindObjectsSortMode.None).ToList();
                for (int i = 0; i < tempRoads.Count; i++)
                {
                    tempRoads[i].isInsidePrefab = GleyPrefabUtilities.IsInsidePrefab(tempRoads[i].gameObject);
                    if (tempRoads[i].isInsidePrefab)
                    {
                        tempRoads[i].positionOffset = GleyPrefabUtilities.GetInstancePrefabRoot(tempRoads[i].gameObject).transform.position;
                        tempRoads[i].rotationOffset = GleyPrefabUtilities.GetInstancePrefabRoot(tempRoads[i].gameObject).transform.localEulerAngles;
                    }
                }
            }

            //verifications
            for (int i = tempRoads.Count - 1; i >= 0; i--)
            {
                if (tempRoads[i].path == null || tempRoads[i].path.NumPoints < 4)
                {
                    DestroyImmediate(tempRoads[i].gameObject);
                    tempRoads.RemoveAt(i);
                    continue;
                }

                if (!tempRoads[i].VerifyAssignments())
                {
                    hasErrors = true;
                }

                tempRoads[i].startPosition = tempRoads[i].path[0] + tempRoads[i].positionOffset;
                tempRoads[i].endPosition = tempRoads[i].path[tempRoads[i].path.NumPoints - 1] + tempRoads[i].positionOffset;
            }

            allRoads = tempRoads.ToArray();
        }
    }
}
