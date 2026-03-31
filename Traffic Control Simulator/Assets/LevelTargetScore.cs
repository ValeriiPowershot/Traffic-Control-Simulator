using UnityEngine;

[DisallowMultipleComponent]
public class LevelTargetScore : MonoBehaviour
{
    [SerializeField] private int _targetScore;

    public int GetTargetScore() =>
        _targetScore;
}
