using BaseCode.Logic;

namespace BaseCode.Interfaces
{
    public interface IScoringObject
    {
        public void Initialize(ScoringManager manager);
        public void Calculate(float deltaTime);
        public bool IsActive();
        public void OnReachedDestination(bool isLostScore);
    }
}
