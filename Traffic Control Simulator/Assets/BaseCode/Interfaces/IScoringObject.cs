using BaseCode.Logic.ScoringSystem;

public interface IScoringObject
{
    public void Initialize(ScoringManager manager);
    public void Calculate(float deltaTime);
}
