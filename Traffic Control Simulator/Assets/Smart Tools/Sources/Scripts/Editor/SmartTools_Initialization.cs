using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.Events;
using UnityEditor.ShortcutManagement;
using UnityEditor.SceneManagement;

namespace SmartTools
{
    public partial class SmartTools
    {
        static void Initialize()
        {
            //Debug.Log("Initialize");
            LoadSettings();
            SetupLayout();
            SetupPresetStrings();
            SetupRoot();
            InitToolTips();
            RegisterEvents();
            GetSceneData();
        }

        static void Deinitialize()
        {
            //Debug.Log("Deinitialize");
            UnregisterEvents();
        }

        static void LoadSettings(bool forceReload = false)
        {
            string[] guids = AssetDatabase.FindAssets("t:SmartTools_Settings");
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settings = AssetDatabase.LoadAssetAtPath<SmartTools_Settings>(path);
            c = settings.currentColorScheme;

            //if (settings == null || forceReload)
            //{
            //string path = SmartTools_Settings.installPath + "/Settings.asset"; //"Assets/Smart Tools/Settings.asset";
            //settings = AssetDatabase.LoadAssetAtPath(path, typeof(SmartTools_Settings)) as SmartTools_Settings;
            //c = settings.currentColorScheme;
            //}
        }

        static void SetupPresetStrings()
        {
            string cBG = "<color=#" + ColorUtility.ToHtmlStringRGB(c.background) + ">" + uc_Preset + "</color>";
            string cCm = "<color=#" + ColorUtility.ToHtmlStringRGB(c.storageCam) + ">" + uc_Preset + "</color>";
            string cSl = "<color=#" + ColorUtility.ToHtmlStringRGB(c.storageSel) + ">" + uc_Preset + "</color>";
            s00 = cBG + "" + cBG;
            s01 = cBG + "" + cSl;
            s10 = cCm + "" + cBG;
            s11 = cCm + "" + cSl;
        }
    }
}
