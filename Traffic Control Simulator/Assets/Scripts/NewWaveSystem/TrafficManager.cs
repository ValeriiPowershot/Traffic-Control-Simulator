using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    private readonly List<Car> _cars = new();

    public int ActiveCars => _cars.Count;

    public void Register(Car car)
    {
        if (!_cars.Contains(car))
            _cars.Add(car);
    }

    public void Unregister(Car car)
    {
        _cars.Remove(car);
    }
}
