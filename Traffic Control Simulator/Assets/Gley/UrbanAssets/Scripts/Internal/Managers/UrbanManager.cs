using Unity.Collections;
using UnityEngine;

#if GLEY_PEDESTRIAN_SYSTEM || GLEY_TRAFFIC_SYSTEM
using Unity.Mathematics;
#endif

namespace Gley.UrbanAssets.Internal
{
    public class UrbanManager : MonoBehaviour
    {
        private GridManager gridManager;

        internal GridManager GridManager
        {
            get
            {
                if (gridManager == null)
                {
                    Debug.LogWarning("Grid manager is null");
                }
                return gridManager;
            }
        }

#if GLEY_PEDESTRIAN_SYSTEM || GLEY_TRAFFIC_SYSTEM
        protected void AddGridManager(CurrentSceneData currentSceneData, NativeArray<float3> activeCameraPositions)
        {
            gridManager = currentSceneData.gameObject.GetComponent<GridManager>();
            if (gridManager == null)
            {
                gridManager = currentSceneData.gameObject.AddComponent<GridManager>().Initialize(currentSceneData, activeCameraPositions);
            }
        }
#endif
    }
}