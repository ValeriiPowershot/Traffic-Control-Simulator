using System;
using System.Collections;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Lights
{
    [Serializable]
    public class VehicleTurnLights 
    {
        public VehicleBase vehicleBase;
        
        public MeshRenderer _leftFrontTurnLight;
        public MeshRenderer _rightFrontTurnLight;
        public MeshRenderer _leftRearTurnLight;
        public MeshRenderer _rightRearTurnLight;

        public Material _turnMaterial;
        public Material _defaultMaterial;

        private Coroutine _leftTurnCoroutine;
        private Coroutine _rightTurnCoroutine;
        private bool _isLeftTurning = false;
        private bool _isRightTurning = false;

        public void CheckTurnLightState()
        {
            if (VehicleController.VehicleLightController.NeedToTurn)
            {
                float rotationY = VehicleReferenceController.arrowIndicatorEndPoint.localRotation.eulerAngles.y;
            
                if (rotationY > 180) 
                    rotationY -= 360;
            
                SetLightState(rotationY);
            }
        }
    
        private void SetLightState(float rotationY)
        {
            switch (rotationY)
            {
                case > 20:
                    ShowTurnLight(Indicator.Right);
                    break;
                case < -20:
                    ShowTurnLight(Indicator.Left);
                    break;
                default:
                    StopTurnSignals();
                    break;
            }
        }

        public void ShowTurnLight(Indicator indicator)
        {
            switch (indicator)
            {
                case Indicator.Left:
                    if (!_isLeftTurning)
                        LeftTurn();
                    break;
                case Indicator.Right:
                    if (!_isRightTurning)
                        RightTurn();
                    break;
                default:
                    Debug.LogWarning("Unknown indicator: " + indicator);
                    break;
            }
        }

        public void LightsBlinking()
        {
            vehicleBase.StartCoroutine(TurnMeshLoop(_leftFrontTurnLight, _leftRearTurnLight));
            vehicleBase.StartCoroutine(TurnMeshLoop(_rightFrontTurnLight, _rightRearTurnLight));
        }
        
        private void LeftTurn()
        {
            StopRightTurn();
            _isLeftTurning = true;
            if (_leftTurnCoroutine != null)
                vehicleBase.StopCoroutine(_leftTurnCoroutine);
            _leftTurnCoroutine = vehicleBase.StartCoroutine(TurnMeshLoop(_leftFrontTurnLight, _leftRearTurnLight));
        }

        private void RightTurn()
        {
            StopLeftTurn();
            _isRightTurning = true;
            if (_rightTurnCoroutine != null)
                vehicleBase.StopCoroutine(_rightTurnCoroutine);
            _rightTurnCoroutine = vehicleBase.StartCoroutine(TurnMeshLoop(_rightFrontTurnLight, _rightRearTurnLight));
        }

        private IEnumerator TurnMeshLoop(MeshRenderer frontMesh, MeshRenderer rearMesh)
        {
            while (true)
            {
                frontMesh.material = _turnMaterial;
                rearMesh.material = _turnMaterial;
                yield return new WaitForSeconds(0.2f);

                frontMesh.material = _defaultMaterial;
                rearMesh.material = _defaultMaterial;
                yield return new WaitForSeconds(0.2f);
            }
        }

        public void StopTurnSignals()
        {
            StopLeftTurn();
            StopRightTurn();
        }

        private void StopLeftTurn()
        {
            if (_leftTurnCoroutine != null)
            {
                vehicleBase.StopCoroutine(_leftTurnCoroutine);
                _leftTurnCoroutine = null;
            }
            _isLeftTurning = false;
            _leftFrontTurnLight.material = _defaultMaterial;
            _leftRearTurnLight.material = _defaultMaterial;
        }

        private void StopRightTurn()
        {
            if (_rightTurnCoroutine != null)
            {
                vehicleBase.StopCoroutine(_rightTurnCoroutine);
                _rightTurnCoroutine = null;
            }
            _isRightTurning = false;
            _rightFrontTurnLight.material = _defaultMaterial;
            _rightRearTurnLight.material = _defaultMaterial;
        }
        private VehicleReferenceController VehicleReferenceController =>
            VehicleController.vehicleReferenceController;
        private VehicleController VehicleController => vehicleBase.vehicleController;
    }

    public enum Indicator
    {
        Left,
        Right,
    }
}