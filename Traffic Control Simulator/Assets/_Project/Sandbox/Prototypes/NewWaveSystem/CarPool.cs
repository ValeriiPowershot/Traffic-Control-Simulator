using System.Collections.Generic;
using UnityEngine;

public class CarPool : MonoBehaviour
{
    [SerializeField] private Car _prefab;
    [SerializeField] private int _initialSize = 10;

    private readonly Stack<Car> _pool = new();

    private void Awake()
    {
        for (int i = 0; i < _initialSize; i++)
        {
            CreateNew();
        }
    }

    private Car CreateNew()
    {
        var car = Instantiate(_prefab, transform);
        car.gameObject.SetActive(false);
        _pool.Push(car);
        return car;
    }

    public Car Get()
    {
        if (_pool.Count > 0)
        {
            var car = _pool.Pop();
            car.gameObject.SetActive(true);
            return car;
        }

        return CreateNew();
    }

    public void Release(Car car)
    {
        car.gameObject.SetActive(false);
        _pool.Push(car);
    }
}
