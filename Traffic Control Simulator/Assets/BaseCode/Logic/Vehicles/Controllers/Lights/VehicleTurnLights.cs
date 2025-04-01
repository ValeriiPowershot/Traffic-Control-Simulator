using System.Collections;
using UnityEngine;

namespace BaseCode.Logic.Vehicles.Controllers.Lights
{
    public class VechicleTurnLights : MonoBehaviour
    {
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

        private void LeftTurn()
        {
            StopRightTurn();
            _isLeftTurning = true;
            if (_leftTurnCoroutine != null)
                StopCoroutine(_leftTurnCoroutine);
            _leftTurnCoroutine = StartCoroutine(TurnMeshLoop(_leftFrontTurnLight, _leftRearTurnLight));
        }

        private void RightTurn()
        {
            StopLeftTurn();
            _isRightTurning = true;
            if (_rightTurnCoroutine != null)
                StopCoroutine(_rightTurnCoroutine);
            _rightTurnCoroutine = StartCoroutine(TurnMeshLoop(_rightFrontTurnLight, _rightRearTurnLight));
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
                StopCoroutine(_leftTurnCoroutine);
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
                StopCoroutine(_rightTurnCoroutine);
                _rightTurnCoroutine = null;
            }
            _isRightTurning = false;
            _rightFrontTurnLight.material = _defaultMaterial;
            _rightRearTurnLight.material = _defaultMaterial;
        }
    }

    public enum Indicator
    {
        Left,
        Right,
    }
}