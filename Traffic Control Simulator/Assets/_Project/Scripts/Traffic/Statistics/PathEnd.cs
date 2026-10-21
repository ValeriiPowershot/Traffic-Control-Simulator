using BaseCode;
using BaseCode.Logic.City;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;

[DisallowMultipleComponent]
public class PathEnd : MonoBehaviour
{
    [SerializeField] private ScoreText _scoreText;
    private readonly System.Collections.Generic.HashSet<int> completed = new System.Collections.Generic.HashSet<int>();
    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponentInParent<RTC_CarController>();
        var value = other.GetComponentInParent<CarScoreValue>();
        var alternate = other.GetComponentInParent<CarScoreValue1>();
        if (controller == null && value == null && alternate == null) return;
        GameObject car = controller != null ? controller.gameObject : value != null ? value.gameObject : alternate.gameObject;
        var director = GameManager.ForScene(gameObject.scene);
        if (director != null)
        {
            if (!director.RecordExit(car)) return;
            if (_scoreText != null) _scoreText.UpdateScoreText(director.Tracker.Statistics.Score);
        }
        else
        {
            if (!completed.Add(car.GetInstanceID())) return;
            if (ScoreData.Instance != null)
            {
                ScoreData.Instance.Score += value != null ? value.CarScore : alternate != null ? alternate.CarScore : 1;
                if (_scoreText != null) _scoreText.UpdateScoreText(ScoreData.Instance.Score);
            }
        }
        Destroy(car);
    }
}
