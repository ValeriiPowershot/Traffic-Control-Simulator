using BaseCode.Logic.Services.Interfaces.Car;

namespace BaseCode.Logic.Entity
{
    public interface ICar
    {
        public ICarLightService CarLightService { get; set; }
        public IPathFindingService PathContainerService { get; set; }
    }
    
}