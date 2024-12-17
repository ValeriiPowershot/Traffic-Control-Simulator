using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    private BasicLight _light;

    private void Awake()
    {
        _light = GetComponentInChildren<BasicLight>();
    }

    public void Interact()
    {
        _light.ChangeLight();
    }
}
