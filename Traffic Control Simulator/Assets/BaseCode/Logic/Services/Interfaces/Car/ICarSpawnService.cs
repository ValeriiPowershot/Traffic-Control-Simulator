namespace BaseCode.Logic.Services.Interfaces.Car
{
    public interface ICarSpawnService
    {
        public void Initialize(CarManager carManager);

        public void SpawnNewCar();
        public void Update();
        public CarManager CarManager { get; set; }

    }
}