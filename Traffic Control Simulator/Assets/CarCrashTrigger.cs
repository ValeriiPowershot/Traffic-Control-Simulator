using System;
using BaseCode.Infrastructure;
using BaseCode.Logic.Vehicles.Controllers;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

[DisallowMultipleComponent]
public class CarCrashTrigger : MonoBehaviour
{
    public Action OnCarCrashed;
    private VehicleController VehicleController; // Ссылка на VehicleController

    // Этот метод инициализирует ссылку на VehicleController
    public void Initialize(VehicleController vehicleController)
    {
        VehicleController = vehicleController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Car))
        {
            VehicleBase hitVehicleBase = other.GetComponent<VehicleBase>();

            // Проверка на null перед использованием hitVehicleBase
            if (hitVehicleBase != null && hitVehicleBase.vehicleController != null)
            {
                // Проверка на разные спавнеры, чтобы обработать только машины с разными спавнерами
                if (AreTheyFromAnotherSpawner(hitVehicleBase))
                {
                    OnCarCrashed?.Invoke();
                }
            }
            else
            {
                Debug.LogWarning("Hit vehicle or its VehicleController is null.");
            }
        }
    }

    private bool AreTheyFromAnotherSpawner(VehicleBase hitVehicleBase)
    {
        // Проверка на null для VehicleController
        if (VehicleController == null)
        {
            Debug.LogError("VehicleController is not initialized in CarCrashTrigger.");
            return false;
        }

        return hitVehicleBase.vehicleController.VehicleCollisionController.spawnIndex != 
            VehicleController.VehicleCollisionController.spawnIndex;
    }
}
