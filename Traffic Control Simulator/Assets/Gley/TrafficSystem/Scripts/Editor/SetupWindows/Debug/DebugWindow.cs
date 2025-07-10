using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using UnityEditor;

namespace Gley.TrafficSystem.Editor
{
    public class DebugWindow : SetupWindowBase
    {
        DebugSettings save;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            save = DebugOptions.LoadOrCreateDebugSettings();
            return base.Initialize(windowProperties,window);
        }


        protected override void TopPart()
        {
            save.debug = EditorGUILayout.Toggle("Debug Vehicle Actions", save.debug);
            if(save.debug == false)
            {
                save.debugSpeed = false;

            }
            save.debugSpeed = EditorGUILayout.Toggle("Debug Vehicle Speed", save.debugSpeed);
            if(save.debugSpeed==true)
            {
                save.debug = true;
            }

            save.debugIntersections = EditorGUILayout.Toggle("Debug Intersections", save.debugIntersections);
            save.debugWaypoints = EditorGUILayout.Toggle("Debug Waypoints", save.debugWaypoints);
            save.debugDisabledWaypoints = EditorGUILayout.Toggle("Disabled Waypoints", save.debugDisabledWaypoints);
            save.drawBodyForces = EditorGUILayout.Toggle("Draw Body Force", save.drawBodyForces);
            save.debugDensity = EditorGUILayout.Toggle("Debug Density", save.debugDensity);
            save.debugPathFinding = EditorGUILayout.Toggle("Debug Path Finding", save.debugPathFinding);
            if(save.debugPathFinding == true)
            {
                save.debug = true;
            }
            base.TopPart();
        }


        public override void DestroyWindow()
        {
            base.DestroyWindow();
            EditorUtility.SetDirty(save);
        }
    }
}
