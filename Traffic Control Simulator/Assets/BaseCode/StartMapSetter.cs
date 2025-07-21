using BaseCode.Logic.Managers;
using BaseCode.Logic.PopUps.PopUp_Base;
using UnityEngine;

public class StartMapSetter : PopUpGameBase
{
    [SerializeField] private GameObject[] _startMaps;
    
    private CarManager CarManager => GameManager.carManager;

    private int CurrentLevelIndex 
        => CarManager.CarSpawnServiceHandler.CarWaveController.CurrentLevelIndex;

    private void Start()
    {
        TurnOffAllObjects();
        _startMaps[CurrentLevelIndex].SetActive(true);
    }

    private void TurnOffAllObjects()
    {
        foreach (GameObject mapObject in _startMaps)
            mapObject.SetActive(false);
    }
}