using System;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class CarInARowCheck : MonoBehaviour
{
    [SerializeField] private int _maxCarsInARow;
    [SerializeField] private TMP_Text _carsInARowText;

    public int CarInARowCount { get; private set; }

    private event Action OnCarInARowChanged;

    // private void OnEnable()
    // {
    //     OnCarInARowChanged += UpdateCarsInARowText;
    // }
    //
    // private void OnDisable()
    // {
    //     OnCarInARowChanged -= UpdateCarsInARowText;
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            CarInARowCount++;
            OnCarInARowChanged?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            CarInARowCount--;
            OnCarInARowChanged?.Invoke();
        }
    }

    private void Update()
    {
        _carsInARowText.text = $"{CarInARowCount}/{_maxCarsInARow}";

    }

    private void UpdateCarsInARowText()
    {
    }

    public bool IsMaxCarsInRowReached()
    {
        return CarInARowCount >= _maxCarsInARow;
    }
}
