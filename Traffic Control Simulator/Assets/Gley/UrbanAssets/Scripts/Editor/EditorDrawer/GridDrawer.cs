using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class GridDrawer : Drawer
    {
        GridData gridData;

        internal GridDrawer Initialize(GridData gridData)
        {
            this.gridData = gridData;
            base.Initialize(gridData);
            return this;
        }


        internal void DrawGrid(bool traffic)
        {
            var grid = gridData.GetGrid();
            int columnLength = grid.Length;
            if (columnLength <= 0)
                return;
            int rowLength = grid[0].row.Length;

            UpdateInViewPropertyForGrid(grid, columnLength, rowLength);

            bool green = false;
            Handles.color = Color.white;
            for (int i = 0; i < columnLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (grid[i].row[j].HasWaypoints(traffic))
                    {
                        if (green == false)
                        {
                            green = true;
                            Handles.color = Color.green;
                        }
                    }
                    else
                    {
                        if (green == true)
                        {
                            green = false;
                            Handles.color = Color.white;
                        }
                    }

                    if (grid[i].row[j].inView)
                    {
                        Handles.DrawWireCube(grid[i].row[j].center, grid[i].row[j].size);
                    }
                }
            }
        }


        private void UpdateInViewPropertyForGrid(GridRow[] grid, int columnLength, int rowLength)
        {
            GleyUtilities.SetCamera();
            if (cameraMoved)
            {
                cameraMoved = false;
                for (int i = 0; i < columnLength; i++)
                {
                    for (int j = 0; j < rowLength; j++)
                    {
                        if (GleyUtilities.IsPointInView(grid[i].row[j].center))
                        {
                            grid[i].row[j].inView = true;
                        }
                        else
                        {
                            grid[i].row[j].inView = false;
                        }
                    }
                }
            }
        }
    }
}