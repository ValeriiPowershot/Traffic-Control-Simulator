using BaseCode.Logic.EntityHandler.Vehicles;
using BaseCode.Logic.Services.Interfaces.Light;

namespace BaseCode.Logic.EntityHandler.Lights.LightControlPointHandler
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