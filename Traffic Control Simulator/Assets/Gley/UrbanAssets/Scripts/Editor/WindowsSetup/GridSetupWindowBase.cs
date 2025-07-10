using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class GridSetupWindowBase : SetupWindowBase
    {
        private Color oldColor;
        protected bool viewGrid;
        private GridData gridData;
        protected GridDrawer gridDrawer;
        private GridCreator gridCreator;
        private int gridCellSize;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            gridData =CreateInstance<GridData>().Initialize();
            gridCreator = CreateInstance<GridCreator>().Initialize(gridData);
            gridDrawer = CreateInstance<GridDrawer>().Initialize(gridData);
            gridCellSize = gridData.GetGridCellSize();
            return base.Initialize(windowProperties, window);
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("The grid is used to improve the performance. Moving agents are generated in the cells adjacent to player cell.\n\n" +
                "The cell size should be smaller if your player speed is low and should increase if your speed is high.\n\n" +
                "You can experiment with this settings until you get the result you want.");
        }


        protected override void ScrollPart(float width, float height)
        {
            gridCellSize = EditorGUILayout.IntField("Grid Cell Size: ", gridCellSize);
            if (GUILayout.Button("Regenerate Grid"))
            {
                gridCreator.GenerateGrid(gridCellSize);
            }
            EditorGUILayout.Space();

            oldColor = GUI.backgroundColor;
            if (viewGrid == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Grid"))
            {
                viewGrid = !viewGrid;
                SceneView.RepaintAll();
            }
            GUI.backgroundColor = oldColor;
            base.ScrollPart(width, height);
        }

        public override void DestroyWindow()
        {
            DestroyImmediate(gridData);
            DestroyImmediate(gridCreator);
            DestroyImmediate(gridDrawer);
        }
    }
}
