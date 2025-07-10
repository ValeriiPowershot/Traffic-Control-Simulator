using Gley.UrbanAssets.Editor;
namespace Gley.TrafficSystem.Editor
{   
    public static class AllWindowsData
    {
        const string urbanNamespace = "GleyUrbanAssets";
        static WindowProperties[] allWindows =
        {
            //main menu
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(MainMenuWindow),"Traffic Settings",false,true,false,true,true,false,"https://youtube.com/playlist?list=PLKeb94eicHQtyL7nYgZ4De1htLs8lmz9C"),
            new WindowProperties(urbanNamespace,nameof(ImportPackagesWindow),"Import Packages",true,true,true,false,true,false,"https://youtu.be/hjKXg6HtWPI"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(RoadSetupWindow),"Road Setup",true,true,true,false,false,false,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(WaypointSetupWindow),"Waypoint Setup",true,true,false,true,true,false,"https://youtu.be/mKfnm5_QW8s"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(SceneSetupWindow), "Scene Setup",true,true,false,true,false,false,"https://youtu.be/203UgxPlfNo"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ExternalToolsWindow), "External Tools",true,true,true,false,false,false,"https://youtu.be/203UgxPlfNo"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(DebugWindow), "Debug",true,true,true,false,false,false,"https://youtu.be/Bg-70Tum380"),

            //Road Setup
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(CreateRoadWindow), "Create Road",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ConnectRoadsWindow), "Connect Roads",true,true,true,true,true,true,"https://youtu.be/EKTVqvYQ01A"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ViewRoadsWindow), "View Roads",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(EditRoadWindow), "Edit Road",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),

            //Waypoint Setup
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowAllWaypoints), "All Waypoints",true,true,true,false,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowVehicleTypeEditedWaypoints), "Vehicle Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowDisconnectedWaypoints), "Disconnected Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowGiveWayWaypoints), "Give Way Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowComplexGiveWayWaypoints), "Complex Give Way Waypoints",true,true,true,true,true,true,""),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowZipperGiveWayWaypoints), "Zipper Give Way Waypoints",true,true,true,true,true,true,""),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowEventWaypoints), "Event Waypoints",true,true,true,true,true,true,""),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowSpeedEditedWaypoints), "Speed Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowPriorityEditedWaypoints), "Priority Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(ShowVehiclePathProblems), "Path Problems",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(EditWaypointWindow), "Edit Waypoint",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),

             //Scene Setup
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(GridSetupWindow), "Grid Setup",true,true,true,true,true,true,"https://youtu.be/203UgxPlfNo"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(SpeedRoutesSetupWindow), "Speed Routes",true,true,false,true,true,true,"https://youtu.be/WqrADi8mUcI"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(VehicleRoutesSetupWindow), "Vehicle Routes",true,true,false,true,true,true,"https://youtu.be/JNVwL9hcodw"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(WaypointPriorityWindow), "Waypoint Priority",true,true,false,true,true,true,"https://youtu.be/JNVwL9hcodw"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(PathFindingWindow), "Path Finding",true,true,true,true,true,true,"https://youtu.be/JNVwL9hcodw"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(LayerSetupWindow), "Layer Setup",true,true,true,false,true,false,"https://youtu.be/203UgxPlfNo"),

             //Intersection
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(IntersectionSetupWindow), "Intersection Setup",true,true,true,true,true,true,"https://youtu.be/iSIE28UoAyY"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(PriorityIntersectionWindow), "Priority Intersection",true,true,true,true,true,true,"https://youtu.be/iSIE28UoAyY"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(TrafficLightsIntersectionWindow), "Traffic Lights Intersection",true,true,true,true,true,true,"https://youtu.be/8tOnYiIYxeU"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(TrafficLightsCrossingWindow), "Traffic Lights Crossing",true,true,true,true,true,true,""),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(PriorityCrossingWindow), "Priority Crossing",true,true,true,true,true,true,""),

             //Car setup
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(VehicleTypesWindow), "Vehicle Types",true,true,true,true,true,false,"https://youtu.be/203UgxPlfNo"),

             //External Tools
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(EasyRoadsSetup), "Easy Roads Setup",true,true,true,true,false,false,"https://youtu.be/203UgxPlfNo"),
             new WindowProperties(Internal.Constants.trafficNamespaceEditor,nameof(CidySetup), "Cidy Setup",true,true,true,true,false,false,"https://youtu.be/203UgxPlfNo"),
        };


        internal static WindowProperties[] GetWindowsData()
        {
            return allWindows;
        }
    }
}
