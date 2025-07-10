using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class SettingsWindowData : ScriptableObject
    {
        public EditorColors editorColors;
        public RoutesColors agentRoutes;
        public RoutesColors pathFindingRoutes;
        public RoutesColors priorityRoutes;

        public bool showConnections;
        public bool showPriority;
        public bool showVehicles;

        public bool viewOtherRoads;
        public bool viewRoadLanes;
        public bool viewRoadWaypoints;
        public bool viewLabels;

        public bool pathFindingEnabled;

        public float waypointDistance;
        public float laneWidth;

        public MoveTools moveTool = MoveTools.Move2D;

        internal abstract SettingsWindowData Initialize();
    }

    [System.Serializable]
    public class EditorColors
    {
        public Color labelColor = Color.white;

        public Color roadColor = Color.green;
        public Color laneColor = Color.blue;
        public Color laneChangeColor = Color.magenta;
        public Color connectorLaneColor = Color.cyan;

        public Color anchorPointColor = Color.white;
        public Color controlPointColor = Color.red;
        public Color roadConnectorColor = Color.cyan;
        public Color selectedRoadConnectorColor = Color.green;

        public Color waypointColor = Color.blue;
        public Color selectedWaypointColor = Color.green;
        public Color disconnectedColor = Color.red;
        public Color prevWaypointColor = Color.yellow;

        public Color speedColor = Color.white;
        public Color agentColor = Color.green;
        public Color priorityColor = Color.green;

        public Color complexGiveWayColor = Color.black;

        public Color intersectionColor = Color.green;
        public Color lightsColor = Color.cyan;
        public Color stopWaypointsColor = Color.red;
        public Color exitWaypointsColor = Color.cyan;
    }

    [System.Serializable]
    public class RoutesColors
    {
        public List<Color> routesColor = new List<Color> { Color.white };
        public List<bool> active = new List<bool> { true };
    }
}
