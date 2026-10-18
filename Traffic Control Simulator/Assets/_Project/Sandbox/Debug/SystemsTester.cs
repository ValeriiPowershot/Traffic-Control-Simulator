using BaseCode.Logic.City;
using UnityEngine;

public class SystemsTester : MonoBehaviour
{
    [SerializeField] private SaveSystem saveSystem;


    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Plus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AddReputation();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.Minus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            RemoveReputation();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            ShowData();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.R))
        {
            ResetSave();
        }
    }


    private void AddReputation()
    {
        saveSystem.Data.Reputation += 5;

        saveSystem.Data.Reputation =
            Mathf.Clamp(saveSystem.Data.Reputation, 0, 100);

        saveSystem.Save();

        Debug.Log($"+5 Reputation. Current: {saveSystem.Data.Reputation}");
    }


    private void RemoveReputation()
    {
        saveSystem.Data.Reputation -= 5;

        saveSystem.Data.Reputation =
            Mathf.Clamp(saveSystem.Data.Reputation, 0, 100);

        saveSystem.Save();

        Debug.Log($"-5 Reputation. Current: {saveSystem.Data.Reputation}");
    }


    private void ShowData()
    {
        Debug.Log(
            $"===== SAVE DATA =====\n" +
            $"Day: {saveSystem.Data.CurrentDay}\n" +
            $"Reputation: {saveSystem.Data.Reputation}\n" +
            $"Results: {saveSystem.Data.Results.Count}"
        );
    }


    private void ResetSave()
    {
        saveSystem.ResetSave();

        Debug.Log("Save reset!");
    }
}
