using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class TurnSignalsSystem : MonoBehaviour
{
    [SerializeField] private MeshRenderer _leftTurnSignal;
    [SerializeField] private MeshRenderer _rightTurnSignal;

    [Space(10)]
    [SerializeField] private Color _turnSignalColor;
    private Color _defaultColor;

    public bool NeedToTurnOnSignal;

    private void Start() =>
        _defaultColor = _leftTurnSignal.material.color;

    public void TurnOnLeftTurnSignal() =>
        StartCoroutine(TurnOnSignal(_leftTurnSignal));

    public void TurnOnRightTurnSignal() =>
        StartCoroutine(TurnOnSignal(_rightTurnSignal));

    public void TurnOffAllSignals()
    {
        NeedToTurnOnSignal = false;
        _leftTurnSignal.material.color = _defaultColor;
        _rightTurnSignal.material.color = _defaultColor;
    }

    private IEnumerator TurnOnSignal(MeshRenderer signal)
    {
        NeedToTurnOnSignal = true;
        
        while (NeedToTurnOnSignal)
        {
            signal.material.color = _turnSignalColor;
            yield return new WaitForSeconds(0.3f);
            signal.material.color = _defaultColor;
            yield return new WaitForSeconds(0.3f);
        }
    }
}
