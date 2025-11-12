using System;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;

[DisallowMultipleComponent]
public class CheckCarCollision : MonoBehaviour
{
    [SerializeField] private RTC_CarController _carController;

    private int _carSpawnIndex;

    private void Start() =>
        _carSpawnIndex = _carController.CarSpawnIndex;

    private void OnCollisionEnter(Collision other)
    {
        int carSpawnIndex = other.transform.GetComponent<RTC_CarController>().CarSpawnIndex;

        Debug.Log(other.transform);
        
        if ( _carSpawnIndex != carSpawnIndex)
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
    }
}
