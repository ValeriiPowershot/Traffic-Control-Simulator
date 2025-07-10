using Gley.UrbanAssets.Editor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ShowVehiclePathProblems : ShowWaypointsTrafficBase
    {
        private bool waypointsLoaded = false;

        public override void DrawInScene()
        {
            waypointsOfInterest = trafficWaypointDrawer.ShowVehiclePathProblems(editorSave.editorColors.waypointColor, editorSave.editorColors.agentColor);

            if (waypointsLoaded == false)
            {
                SettingsWindowBase.TriggerRefreshWindowEvent();
                waypointsLoaded = true;
            }
            base.DrawInScene();
        }

        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }
    }
}
