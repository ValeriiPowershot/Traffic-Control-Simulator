using Gley.UrbanAssets.Editor;

namespace Gley.TrafficSystem.Editor
{
    public class GridSetupWindow : GridSetupWindowBase
    {
        public override void DrawInScene()
        {
            if (viewGrid)
            {
                gridDrawer.DrawGrid(true);
            }
            base.DrawInScene();
        }
    }
}
