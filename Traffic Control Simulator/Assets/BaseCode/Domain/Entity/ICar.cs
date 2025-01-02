using BaseCode.Logic.Services;
using BaseCode.Logic.Services.Interfaces.Car;

namespace BaseCode.Domain.Entity
{
    public interface ICar
    {
        public ICarLightService CarLightService { get; set; }
        public IPathFindingService PathContainerService { get; set; }
    }
    
}