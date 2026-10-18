using BaseCode.Logic.City;
using UnityEngine;

public class SaveTester : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;

    private CityStatistics testStats = new CityStatistics();


    private void Start()
    {
        testStats.CarsPassed = 100;
        testStats.Accidents = 0;

        UpdateScore();

        Debug.Log("TEST CONTROLS:");
        Debug.Log("+  Add cars");
        Debug.Log("-  Remove cars");
        Debug.Log("]  Add accident");
        Debug.Log("[  Remove accident");
        Debug.Log("ENTER  Finish day");
    }


    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Plus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AddCars();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.Minus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            RemoveCars();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.RightBracket))
        {
            AddAccident();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket))
        {
            RemoveAccident();
        }


        if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            FinishDay();
        }
    }


    private void AddCars()
    {
        testStats.CarsPassed += 100;

        UpdateScore();

        Debug.Log($"Cars: {testStats.CarsPassed}");
    }


    private void RemoveCars()
    {
        testStats.CarsPassed -= 100;

        if (testStats.CarsPassed < 0)
            testStats.CarsPassed = 0;


        UpdateScore();

        Debug.Log($"Cars: {testStats.CarsPassed}");
    }


    private void AddAccident()
    {
        testStats.Accidents++;

        UpdateScore();

        Debug.Log($"Accidents: {testStats.Accidents}");
    }


    private void RemoveAccident()
    {
        testStats.Accidents--;

        if (testStats.Accidents < 0)
            testStats.Accidents = 0;


        UpdateScore();

        Debug.Log($"Accidents: {testStats.Accidents}");
    }


    private void FinishDay()
    {
        dayManager.FinishDay(testStats);
    }


    private void UpdateScore()
    {
        testStats.Score =
            testStats.CarsPassed - (testStats.Accidents * 50);


        Debug.Log($"Score: {testStats.Score}");
    }
}
