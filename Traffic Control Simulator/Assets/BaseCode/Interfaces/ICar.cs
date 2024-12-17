
//Interface for all cars to be conrolled by the lights
public interface ICar
{
    //passes light state
    //could be called multiple times to inform about state changes
    public void PassLightState(LightState State);
}

public enum LightState
{
    None,
    Green,
    Red,
}
