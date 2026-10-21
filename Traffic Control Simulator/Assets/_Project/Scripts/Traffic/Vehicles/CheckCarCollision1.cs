using BaseCode.Logic.City;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;

[DisallowMultipleComponent]
public class CheckCarCollision1 : MonoBehaviour
{
    [SerializeField] private RTC_CarController _carController;
    private void OnCollisionEnter(Collision collision)
    {
        var mine = _carController != null ? _carController : GetComponentInParent<RTC_CarController>();
        var other = collision.collider.GetComponentInParent<RTC_CarController>();
        if (mine == null || other == null || mine == other) return;
        var director = GameManager.ForScene(gameObject.scene);
        if (director != null) director.RecordCollision(mine.gameObject, other.gameObject);
    }
}
