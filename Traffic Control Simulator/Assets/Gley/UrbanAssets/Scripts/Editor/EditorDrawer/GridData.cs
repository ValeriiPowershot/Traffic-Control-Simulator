using Gley.UrbanAssets.Internal;

namespace Gley.UrbanAssets.Editor
{
    public class GridData : Data
    {
        CurrentSceneData currentSceneData;

        internal new GridData Initialize()
        {
            base.Initialize();
            return this;
        }


        protected override void LoadAllData()
        {
            currentSceneData = CurrentSceneData.GetSceneInstance();
        }


        internal CurrentSceneData GetCurrentSceneData()
        {
            return currentSceneData;
        }


        internal GridRow[] GetGrid()
        {
            return currentSceneData.grid;
        }


        internal int GetGridCellSize()
        {
            return currentSceneData.gridCellSize;
        }
    }
}