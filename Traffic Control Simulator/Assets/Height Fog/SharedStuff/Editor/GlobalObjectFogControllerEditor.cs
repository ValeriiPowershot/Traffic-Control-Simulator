// SKGames vertical fog editor GUI. Copyright (c) 2018 Sergey Klimenko. 18.05.2018
using UnityEditor;
using System.Linq;
using UnityEngine;

[CustomEditor(typeof(GlobalObjectFogController))]
public class GlobalObjectFogControllerEditor : Editor
{
    private GUIStyle                  boxStyle;
    private GlobalObjectFogController targetInstance;

    public override void OnInspectorGUI()
    {
        boxStyle          = GUI.skin.GetStyle("HelpBox");
        boxStyle.normal.textColor = Color.black;
        boxStyle.richText = true;
        boxStyle.padding  = new RectOffset(10, 10, 10, 10);
        targetInstance    = (GlobalObjectFogController)target;
        if (GlobalObjectFogController.Exists)
        {
            EditorGUILayout.TextArea("This controller overrides: <b>" + 
                                     (targetInstance.controllers != null ? targetInstance.controllers.Count(x => x.overridedFromGlobalController) : 0) +
                                     "</b> items (total fog controllers: <b>" + (targetInstance.controllers != null ? targetInstance.controllers.Count() : 0) + "</b>)", boxStyle);
            if (targetInstance.controllers != null && targetInstance.controllers.Count(x=>x.overridedFromGlobalController) > 0)
            {
                if (GUILayout.Button("Disable all overrides"))
                {
                    for (int i = 0; i < targetInstance.controllers.Count; i++)
                    {
                        targetInstance.controllers[i].overridedFromGlobalController = false;
                        targetInstance.controllers[i].Update();
                    }
                    if (GUI.changed) EditorUtility.SetDirty(target);
                }
            }
            if (targetInstance.controllers != null && targetInstance.controllers.Count(x => x.overridedFromGlobalController) != targetInstance.controllers.Count())
            {
                if (GUILayout.Button("Enable all overrides"))
                {
                    for (int i = 0; i < targetInstance.controllers.Count; i++)
                    {
                        targetInstance.controllers[i].overridedFromGlobalController = true;
                        targetInstance.controllers[i].Update();
                    }
                    if (GUI.changed) EditorUtility.SetDirty(target);
                }
            }
            DrawDefaultInspector();
        }
        else
        {
            EditorGUILayout.TextArea("<b>Global controller disabled</b>", boxStyle);
        }
    }
}
