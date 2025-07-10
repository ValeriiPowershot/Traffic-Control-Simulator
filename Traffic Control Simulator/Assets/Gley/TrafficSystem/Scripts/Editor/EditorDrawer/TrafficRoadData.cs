using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficRoadData : RoadData<Road>
    {
        internal new TrafficRoadData Initialize()
        {
            base.Initialize();
            return this;
        }


        public override Road[] GetAllRoads()
        {
            return allRoads;
        }
    }
}
