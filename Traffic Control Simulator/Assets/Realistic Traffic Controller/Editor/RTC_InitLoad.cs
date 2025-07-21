//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class RTC_InitLoad : EditorWindow {

    [InitializeOnLoadMethod]
    static void InitOnLoad() {

        EditorApplication.delayCall += EditorUpdate;

    }

    public static void EditorUpdate() {

        bool hasKey = false;

#if BCG_RTRC
        hasKey = true;
#endif

        if (!hasKey) {

            EditorUtility.DisplayDialog("Regards from BoneCracker Games", "Thank you for purchasing and using Realistic Traffic Controller. Please read the documentation before use. Also check out the online documentation for updated info. Have fun :)", "Let's get started!");

        }

        RTC_SetScriptingSymbol.SetEnabled("BCG_RTRC", true);
        RTC_Installation.Check();

    }

}
