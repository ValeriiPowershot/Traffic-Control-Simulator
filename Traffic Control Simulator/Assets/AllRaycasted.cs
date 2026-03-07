using System;
using System.Collections;
using System.Collections.Generic;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;

[DisallowMultipleComponent]
public class AllRaycasted : MonoBehaviour
{
    public RTC_CarController _carController;

    public List<GameObject> _allRaycastedObjects;

    private void Update()
    {
        _allRaycastedObjects = _carController.raycast;
    }
}
