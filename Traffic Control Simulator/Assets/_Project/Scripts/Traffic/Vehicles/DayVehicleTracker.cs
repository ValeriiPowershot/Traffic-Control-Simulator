using UnityEngine;
using BaseCode.Logic.City;

public sealed class DayVehicleTracker : MonoBehaviour
{
    private GameManager director;
    private Vector3 previousPosition;
    private float stoppedFor;
    private bool exited;
    public void Initialize(GameManager owner) { director = owner; previousPosition = transform.position; stoppedFor = 0f; exited = false; }
    public bool TryExit() { if (exited) return false; exited = true; return true; }
    private void FixedUpdate()
    {
        if (director == null || !director.IsPlaying || exited) return;
        float speed = Vector3.Distance(transform.position, previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;
        if (speed < 0.5f)
        {
            stoppedFor += Time.fixedDeltaTime;
            if (stoppedFor > 2f) director.Tracker.AddStoppedTime(Time.fixedDeltaTime);
        }
        else stoppedFor = 0f;
    }
}
