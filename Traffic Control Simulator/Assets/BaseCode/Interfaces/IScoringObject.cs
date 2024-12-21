public interface IScoringObject
{
    public void Initialize(ScoringManager manager);
    public void Calculate(float deltaTime);
}
