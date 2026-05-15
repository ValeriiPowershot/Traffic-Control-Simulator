// SKGames vertical fog editor GUI. Copyright (c) 2018 Sergey Klimenko. 18.05.2018
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectFogController))]
public class ObjectFogControllerEditor : Editor
{
    private GUIStyle            boxStyle;
    private ObjectFogController targetInstance;

    public override void OnInspectorGUI()
    {
        boxStyle          = GUI.skin.GetStyle("HelpBox");
        boxStyle.richText = true;
        boxStyle.normal.textColor = new Color(0.8f, 0f, 0f, 1f);
        targetInstance    = (ObjectFogController)target;
        if (targetInstance.overridedFromGlobalController && GlobalObjectFogController.Exists)
        {
            EditorGUILayout.TextArea("<b>This controller overrided from global controller!</b>", boxStyle);
            if (GUILayout.Button("Disable override for this item"))
            {
                targetInstance.overridedFromGlobalController = false;
            }
        }
        else
        {
            DrawDefaultInspector();
            if (!GlobalObjectFogController.Exists)
            {
                EditorGUILayout.TextArea("<b>Global controller not exists or disabled, override can't be applied!</b>", boxStyle);
            }
            else
            {
                if (GUILayout.Button("Enable override for this item"))
                {
                    targetInstance.overridedFromGlobalController = true;
                }
            }
            
        }
    }
}
