using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

#if UNITY_EDITOR
namespace SmartTools
{
    [CreateAssetMenu(fileName = "SmartTools_Settings", menuName = "Smart Tools/Smart Tools Settings", order = 1)]
    public class SmartTools_Settings : ScriptableObject
    {
        static public string installPath;
        static public Texture logo;
        //static public Texture2D grid;

        public int currentColorSchemeIndex;
        [HideInInspector] public ColorScheme currentColorScheme;
        public ColorScheme[] colorSchemes;
        public float[] gridSizePresets = { 0.0625f, 0.125f, 0.25f, 0.5f, 1.0f, 2.0f, 3.0f, 4.0f, 8.0f, 16.0f };
        public float[] incrementPresets = { 0.25f, 0.5f, 1.0f, 2.0f, 3.0f, 4.0f, 8.0f, 16.0f };
        public float[] anglePresets = { 5.625f, 11.25f, 22.5f, 30.0f, 45.0f, 90.0f };
        [Range(0.0f, 1f)] public float _VertexPush;

        [System.Serializable] public class ColorScheme
        {
            [ColorUsage(false)] public Color background;
            [ColorUsage(false)] public Color buttonDefault;
            [ColorUsage(false)] public Color textDefault;
            [ColorUsage(false)] public Color buttonPos;
            [ColorUsage(false)] public Color textPos;
            [ColorUsage(false)] public Color buttonRot;
            [ColorUsage(false)] public Color textRot;
            [ColorUsage(false)] public Color buttonScl;
            [ColorUsage(false)] public Color textScl;
            [ColorUsage(false)] public Color buttonBright;
            [ColorUsage(false)] public Color textBright;
            [ColorUsage(false)] public Color buttonHighlight;
            [ColorUsage(false)] public Color textHighlight;
            [ColorUsage(false)] public Color textX;
            [ColorUsage(false)] public Color textY;
            [ColorUsage(false)] public Color textZ;
            [ColorUsage(false)] public Color buttonCComp;
            [ColorUsage(false)] public Color buttonDark;
            [ColorUsage(false)] public Color storageCam;
            [ColorUsage(false)] public Color storageSel;
            [Min(0)] public float hoverMulti = 1.3f;
            [Range(0, 1)] public float disabledOpacity = 0.4f;
            [Range(0, 8)] public int buttonRadius = 3;
        }

        public void OnValidate()
        {
            installPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            int index = installPath.LastIndexOf("/Sources/Scripts/Editor");
            installPath = installPath.Substring(0, index);
            //logo = AssetDatabase.LoadAssetAtPath<Texture>(installPath + "/Sources/SmartTools_Logo.png");

            var texturePath = Application.dataPath + installPath.Replace("Assets", "") + "/Sources/SmartTools_Logo.bytes";
            logo = LoadLogoTexture(texturePath);

            //var gridPath = Application.dataPath + installPath.Replace("Assets", "") + "/Sources/SmartTools_Grid.bytes";
            //grid = LoadGridTexture(gridPath);

            currentColorScheme = colorSchemes[currentColorSchemeIndex];
            SmartTools.RefreshOverlay();
        }



        public static Texture2D LoadLogoTexture(string path)
        {
            byte[] fileData = System.IO.File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                tex.alphaIsTransparency = true;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.filterMode = FilterMode.Bilinear;
                return tex;
            }
            else
            {
                Debug.LogError("Failed to load texture from path: " + path);
                return null;
            }
        }

        //public static Texture2D LoadGridTexture(string path)
        //{
        //    byte[] fileData = System.IO.File.ReadAllBytes(path);
        //    Texture2D tex = new Texture2D(2, 2);
        //    if (tex.LoadImage(fileData))
        //    {
        //        tex.wrapMode = TextureWrapMode.Repeat;
        //        tex.filterMode = FilterMode.Trilinear;
        //        tex.anisoLevel = 16;
        //        tex.mipMapBias = -0.5f;
        //        return tex;
        //    }
        //    else
        //    {
        //        Debug.LogError("Failed to load texture from path: " + path);
        //        return null;
        //    }
        //}
    }





    [CustomEditor(typeof(SmartTools_Settings))]
    public class SmartTools_SettingsEditor : Editor
    {
        SmartTools_Settings script;

        SerializedProperty gridSizePresets;
        SerializedProperty incrementPresets;
        SerializedProperty anglePresets;
        SerializedProperty _VertexPush;
        SerializedProperty colorSchemes;
        SerializedProperty currentColorSchemeIndex;

        private void OnEnable()
        {
            script = (SmartTools_Settings)target;
            gridSizePresets = serializedObject.FindProperty("gridSizePresets");
            incrementPresets = serializedObject.FindProperty("incrementPresets");
            anglePresets = serializedObject.FindProperty("anglePresets");
            _VertexPush = serializedObject.FindProperty("_VertexPush");
            colorSchemes = serializedObject.FindProperty("colorSchemes");
            currentColorSchemeIndex = serializedObject.FindProperty("currentColorSchemeIndex");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUILayout.Space(10);
            var texScale = 0.5f;// 0.2f;
            float logow = SmartTools_Settings.logo.width * texScale;
            float logoh = SmartTools_Settings.logo.height * texScale;


            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(logoh));
            float rw = r.width;
            r.width = logow;



            GUI.DrawTexture(r, SmartTools_Settings.logo);
            r.x += r.width + 16;
            r.width = rw - logow - 16;

            //EditorGUI.HelpBox(r, "Please do not move, rename, or duplicate this object!\n\nYou may move the whole SmartTools Folder as a whole to another location but please keep the intermal folder structure intact.", MessageType.Warning);
            EditorGUI.HelpBox(r, "Please keep the intermal folder structure intact.", MessageType.Warning);


            GUIContent gc = new GUIContent();
            float lw = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 140;

            if (currentColorSchemeIndex.intValue >= colorSchemes.arraySize) currentColorSchemeIndex.intValue = colorSchemes.arraySize - 1;

            

            //EditorGUILayout.Space(10);
            //EditorGUILayout.HelpBox("Please do not move or duplicate this object! You may move the whole SmartTools Folder as a whole to another location but please keep the intermal folder structure intact.", MessageType.Warning, true);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Current Color Scheme";
            EditorGUILayout.IntSlider(currentColorSchemeIndex, 0, colorSchemes.arraySize - 1, gc);

            EditorGUIUtility.labelWidth = 190;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Colorscheme"))
            {
                colorSchemes.InsertArrayElementAtIndex(colorSchemes.arraySize - 1);
                currentColorSchemeIndex.intValue = colorSchemes.arraySize - 1;
                serializedObject.ApplyModifiedProperties();
            }
            GUI.enabled = currentColorSchemeIndex.intValue != 0;
            if (GUILayout.Button("Delete Colorscheme"))
            {
                colorSchemes.DeleteArrayElementAtIndex(currentColorSchemeIndex.intValue);
                if (currentColorSchemeIndex.intValue >= colorSchemes.arraySize) currentColorSchemeIndex.intValue = colorSchemes.arraySize - 1;
                serializedObject.ApplyModifiedProperties();

            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);


            GUIStyle style = new GUIStyle("Label");
            style.fontSize = 15;
            style.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("Color Scheme " + (currentColorSchemeIndex.intValue == 0 ? "(Default)" : currentColorSchemeIndex.intValue.ToString()), style);
            EditorGUILayout.Space(6);

            GUI.enabled = currentColorSchemeIndex.intValue != 0;
            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Background Color";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("background"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Default Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonDefault"), gc);
            gc.text = "Default Buttons Text";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textDefault"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Position Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonPos"), gc);
            gc.text = "Position Buttons Text";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textPos"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Rotation Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonRot"), gc);
            gc.text = "Rotation Buttons Text";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textRot"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Scale Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonScl"), gc);
            gc.text = "Scale Buttons Text";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textScl"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Priority Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonBright"), gc);
            gc.text = "Priority Buttons Text";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textBright"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Highlighted Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonHighlight"), gc);
            gc.text = "Highlighted Buttons Text";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textHighlight"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Axis X Text Color";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textX"), gc);
            gc.text = "Axis Y Text Color";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textY"), gc);
            gc.text = "Axis Z Text Color";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("textZ"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Child Compensation Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonCComp"), gc);
            gc.text = "Secondary Buttons Background";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonDark"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Stored View Indicators";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("storageCam"), gc);
            gc.text = "Stored Selection Indicators";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("storageSel"), gc);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("HelpBox");
            gc.text = "Button Hover Brightness Multi";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("hoverMulti"), gc);
            gc.text = "Button Disabled Opactiy";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("disabledOpacity"), gc);
            gc.text = "Button Border Radius";
            EditorGUILayout.PropertyField(colorSchemes.GetArrayElementAtIndex(currentColorSchemeIndex.intValue).FindPropertyRelative("buttonRadius"), gc);
            EditorGUILayout.EndVertical();

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
            EditorGUIUtility.labelWidth = lw;

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.LabelField("Other Settings", style);
            EditorGUILayout.Space(6);

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(gridSizePresets);
            EditorGUILayout.PropertyField(incrementPresets);
            EditorGUILayout.PropertyField(anglePresets);
            gc.text = "Anti z-Fight";
            gc.tooltip = "Adjusting this slider is rarely necessary, but if you experience Z-fighting between the grid and scene geometry, you can move the slider to the right. Note: the slider defaults to the far left, which is appropriate for nearly all situations.";
            EditorGUILayout.PropertyField(_VertexPush, gc);
            EditorGUI.indentLevel++;
            EditorGUILayout.EndVertical();



            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
