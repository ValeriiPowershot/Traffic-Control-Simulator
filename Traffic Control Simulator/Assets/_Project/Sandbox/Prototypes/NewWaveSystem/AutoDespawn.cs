using UnityEngine;

public class AutoDespawn : MonoBehaviour
{
    [SerializeField] private float _lifetime = 10f;

    private float _time;
    private Car _car;

    private void Awake()
    {
        _car = GetComponent<Car>();
    }

    private void OnEnable()
    {
        _time = 0f;
    }

    private void Update()
    {
        _time += Time.deltaTime;

        if (_time >= _lifetime)
        {
            _car.Despawn();
        }
    }
}
