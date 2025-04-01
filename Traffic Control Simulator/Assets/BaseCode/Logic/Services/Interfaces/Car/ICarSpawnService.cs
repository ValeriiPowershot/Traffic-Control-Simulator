using BaseCode.Logic.Managers;

namespace BaseCode.Logic.Services.InterfaceHandler.Car
{
    public interface ICarSpawnService
    {
        public void Initialize(CarManager carManager);

        public void Update();
        public CarManager CarManager { get; set; }
    }
}