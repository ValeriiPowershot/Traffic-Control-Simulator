using System;
using BaseCode.Logic.Services.Handler;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers
{
    public class VehicleBlinker : MonoBehaviour
    {
        private float _blinkTime;
        public float time = 0.25f;
        public GameObject blinker;

        public bool isNone;
        public void Update()
        {
            if (isNone)
                return;
            _blinkTime -= Time.deltaTime;
            if (_blinkTime < 0)
            {
                _blinkTime = time;
                blinker.SetActive(!blinker.activeSelf);
            }
        }
    }
}