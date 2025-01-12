namespace BaseCode.Logic.Services.Interfaces.Car
{
    public interface ICarSpawnService
    {
        public void Initialize(CarManager carManager);

        public void Update();
        public CarManager CarManager { get; set; }
    }
}