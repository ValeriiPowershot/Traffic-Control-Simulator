using UnityEngine;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private float _switchDelay;

    private BasicLight _light;
    private float _switchTimer;
    private bool _interactCalled;

    private void Awake()
    {
        _light = GetComponentInChildren<BasicLight>();
    }

    private void Update()
    {
        if(_interactCalled && Time.time > _switchTimer)
        {
            _interactCalled = false;
            _light.ChangeLight();
        }
    }

    public void Interact()
    {
        _interactCalled = true;
        _switchTimer = Time.time + _switchDelay;
    }
}