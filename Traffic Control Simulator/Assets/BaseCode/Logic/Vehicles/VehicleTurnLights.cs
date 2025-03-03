using System.Collections;
using UnityEngine;

public class VechicleTurnLights : MonoBehaviour
{
    public MeshRenderer _leftFrontTurnLight;
    public MeshRenderer _rightFrontTurnLight;
    public MeshRenderer _leftRearTurnLight;
    public MeshRenderer _rightRearTurnLight;

    public Material _turnMaterial;
    public Material _defaultMaterial;

    private bool _isTurning;
    private Coroutine _leftTurnCoroutine;
    private Coroutine _rightTurnCoroutine;

    public void ShowTurnLight(Indicator indicator)
    {
        switch (indicator)
        {
            case Indicator.Left:
                LeftTurn();
                break;
            case Indicator.Right:
                RightTurn();
                break;
            default:
                Debug.LogWarning("Unknown indicator: " + indicator);
                break;
        }
    }

    public void StopTurnSignals()
    {
        _isTurning = false;

        if (_leftTurnCoroutine != null) StopCoroutine(_leftTurnCoroutine);
        if (_rightTurnCoroutine != null) StopCoroutine(_rightTurnCoroutine);

        _leftTurnCoroutine = null;
        _rightTurnCoroutine = null;

        _leftFrontTurnLight.material = _defaultMaterial;
        _rightFrontTurnLight.material = _defaultMaterial;
        _leftRearTurnLight.material = _defaultMaterial;
        _rightRearTurnLight.material = _defaultMaterial;
    }

    private void LeftTurn()
    {
        StopTurnSignals();
        _isTurning = true;
        _leftTurnCoroutine = StartCoroutine(TurnLightLoop(_leftFrontTurnLight, _leftRearTurnLight));
    }

    private void RightTurn()
    {
        StopTurnSignals();
        _isTurning = true;
        _rightTurnCoroutine = StartCoroutine(TurnLightLoop(_rightFrontTurnLight, _rightRearTurnLight));
    }

    private IEnumerator TurnLightLoop(MeshRenderer frontLight, MeshRenderer rearLight)
    {
        while (_isTurning)
        {
            frontLight.material = _turnMaterial;
            rearLight.material = _turnMaterial;
            yield return new WaitForSeconds(0.2f);

            frontLight.material = _defaultMaterial;
            rearLight.material = _defaultMaterial;
            yield return new WaitForSeconds(0.2f);
        }
    }
}

public enum Indicator
{
    Left,
    Right,
}
