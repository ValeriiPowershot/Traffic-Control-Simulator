using Gley.UrbanAssets.Editor;
using UnityEditor;

namespace Gley.TrafficSystem.Editor
{
    public class ShowAllWaypoints : ShowWaypointsTrafficBase
    {
        public override void DrawInScene()
        {
            trafficWaypointDrawer.ShowAllWaypoints(editorSave.editorColors.waypointColor, editorSave.showConnections, editorSave.showSpeed,editorSave.editorColors.speedColor, editorSave.showVehicles, editorSave.editorColors.agentColor, editorSave.showOtherLanes, editorSave.editorColors.laneChangeColor, editorSave.showPriority,editorSave.editorColors.priorityColor);
            base.DrawInScene();
        }
    }
}
