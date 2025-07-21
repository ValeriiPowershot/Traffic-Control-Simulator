//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All RTC demo materials.
/// </summary>
public class RTC_DemoMaterials : ScriptableObject {

    #region singleton
    private static RTC_DemoMaterials instance;
    public static RTC_DemoMaterials Instance { get { if (instance == null) instance = Resources.Load("RTC_DemoMaterials") as RTC_DemoMaterials; return instance; } }
    #endregion

    public Material[] demoMaterials;

}
