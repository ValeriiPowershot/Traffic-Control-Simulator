using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficRoadDrawer : RoadDrawer<TrafficRoadData, Road>
    {
        internal TrafficRoadDrawer Initialize(TrafficRoadData data)
        {
            base.Initialize(data);
            return this;
        }
    }
}
