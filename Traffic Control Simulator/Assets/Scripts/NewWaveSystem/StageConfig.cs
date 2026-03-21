using UnityEngine;

[CreateAssetMenu(menuName = "Traffic/Stage Config")]
public class StageConfig : ScriptableObject
{
    public float StartTime;
    public float EndTime;

    public int MaxCarsOnScreen;
}
