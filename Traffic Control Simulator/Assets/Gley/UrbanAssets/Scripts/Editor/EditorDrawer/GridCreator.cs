using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class GridCreator : Creator
    {
        private GridData gridData;
        private CurrentSceneData currentSceneData;

        internal GridCreator Initialize(GridData gridData)
        {    
            this.gridData = gridData;
            currentSceneData = gridData.GetCurrentSceneData();
            return this;
        }


        internal void GenerateGrid(int gridCellsize)
        {
            System.DateTime startTime = System.DateTime.Now;
            currentSceneData.gridCellSize = gridCellsize;
            int nrOfColumns;
            int nrOfRows;
            Bounds b = new Bounds();
            foreach (Renderer r in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
            {
                b.Encapsulate(r.bounds);
            }
            foreach (Terrain t in FindObjectsByType<Terrain>(FindObjectsSortMode.None))
            {
                b.Encapsulate(t.terrainData.bounds);
            }

            nrOfColumns = Mathf.CeilToInt(b.size.x / currentSceneData.gridCellSize);
            nrOfRows = Mathf.CeilToInt(b.size.z / currentSceneData.gridCellSize);
            if (nrOfRows == 0 || nrOfColumns == 0)
            {
                Debug.LogError("Your scene seems empty. Please add some geometry inside your scene before setting up traffic");
                return;
            }
            Debug.Log("Center: " + b.center + " size: " + b.size + " nrOfColumns " + nrOfColumns + " nrOfRows " + nrOfRows);
            Vector3 corner = new Vector3(b.center.x - b.size.x / 2 + currentSceneData.gridCellSize / 2, 0, b.center.z - b.size.z / 2 + currentSceneData.gridCellSize / 2);
            int nr = 0;
            currentSceneData.grid = new GridRow[nrOfRows];
            for (int row = 0; row < nrOfRows; row++)
            {
                currentSceneData.grid[row] = new GridRow(nrOfColumns);
                for (int column = 0; column < nrOfColumns; column++)
                {
                    nr++;
                    currentSceneData.grid[row].row[column] = new GridCell(column, row, new Vector3(corner.x + column * currentSceneData.gridCellSize, 0, corner.z + row * currentSceneData.gridCellSize), currentSceneData.gridCellSize);
                }
            }
            currentSceneData.gridCorner = currentSceneData.grid[0].row[0].center - currentSceneData.grid[0].row[0].size / 2;
            EditorUtility.SetDirty(currentSceneData);
            gridData.TriggerModifiedEvent();
            Debug.Log("Done generate grid in " + (System.DateTime.Now - startTime));
        }
    }
}
