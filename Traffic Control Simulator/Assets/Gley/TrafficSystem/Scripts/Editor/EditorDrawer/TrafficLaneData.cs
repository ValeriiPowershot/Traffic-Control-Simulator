using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficLaneData : LaneData<Road,WaypointSettings>
    {
        internal TrafficLaneData Initialize(TrafficRoadData roadData)
        {
            base.Initialize(roadData);
            return this;
        }
    }
}