using BaseCode.Logic.Entity.Lights;
using BaseCode.Logic.Entity.Lights.Services;
using BaseCode.Logic.Lights.Services;
using BaseCode.Logic.Vehicles.Vehicles;

namespace BaseCode.Logic.Lights.Handler.Abstracts
{
    public abstract class LightControlPointBase : ILightControlPoint
    {
        protected BasicLight ParentLightControlPoint { get; private set; }

        public void Initialize(BasicLight parentLightControlPoint)
        {
            ParentLightControlPoint = parentLightControlPoint;
        }

        public abstract void OnVehicleEnter(VehicleBase vehicle);
        public abstract void OnVehicleExit(VehicleBase vehicle);
    
    }

}