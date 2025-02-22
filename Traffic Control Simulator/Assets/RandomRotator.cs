using UnityEngine;
using UnityEditor;

public class RandomRotator : MonoBehaviour
{
    void RotateObjects()
    {
        foreach (Transform obj in transform)
        {
            if (obj != null)
            {
                int randomAngleY = Random.Range(0, 4) * 90; // 0, 90, 180, 270
                
                obj.rotation = Quaternion.Euler(0, randomAngleY, 0);
            }
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Regenerate Rotations")]
    public void Regenerate()
    {
        RotateObjects();
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(RandomRotator))]
public class RandomRotatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomRotator script = (RandomRotator)target;
        if (GUILayout.Button("Regenerate Rotations"))
        {
            script.Regenerate();
        }
    }
}
#endif
