//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RTC_Installation {

    public static void Check() {

        bool layer_RTCTrafficVehicles = false;
        bool layer_RTCTrafficLights = false;
        bool layer_RTCGround = false;

        string[] missingLayers = new string[3];

        layer_RTCTrafficVehicles = LayerExists("RTC_TrafficVehicle");
        layer_RTCTrafficLights = LayerExists("RTC_TrafficLight");
        layer_RTCGround = LayerExists("RTC_Ground");

        if (!layer_RTCTrafficVehicles)
            missingLayers[0] = "RTC_TrafficVehicle";

        if (!layer_RTCTrafficLights)
            missingLayers[1] = "RTC_TrafficLight";

        if (!layer_RTCGround)
            missingLayers[2] = "RTC_Ground";

        if (!layer_RTCTrafficVehicles || !layer_RTCTrafficLights || !layer_RTCGround) {

            if (EditorUtility.DisplayDialog("Found Missing Layers For Realistic Traffic Controller", "These layers will be added to the Tags and Layers\n\n" + missingLayers[0] + "\n" + missingLayers[1], "Add")) {

                CheckLayer("RTC_TrafficVehicle");
                CheckLayer("RTC_TrafficLight");
                CheckLayer("RTC_Ground");

            }

        }

        if (RTC_Settings.Instance.trafficLightsLayer != null) {

            foreach (GameObject item in RTC_AssetPaths.Instance.demoVehicles) {

                if (item != null) {

                    item.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficVehiclesLayer);

                    foreach (Transform transform in item.GetComponentsInChildren<Transform>())
                        transform.gameObject.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficVehiclesLayer);

                }

            }

        }

        if (RTC_Settings.Instance.trafficLightsLayer != null) {

            if (RTC_AssetPaths.Instance.trafficLight != null) {

                RTC_AssetPaths.Instance.trafficLight.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficLightsLayer);

                foreach (Transform transform in RTC_AssetPaths.Instance.trafficLight.GetComponentsInChildren<Transform>())
                    transform.gameObject.layer = LayerMask.NameToLayer(RTC_Settings.Instance.trafficLightsLayer);

            }

        }

    }

    public static bool CheckTag(string tagName) {

        if (TagExists(tagName))
            return true;

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName)) {

            int index = tagsProp.arraySize;

            tagsProp.InsertArrayElementAtIndex(index);
            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);

            sp.stringValue = tagName;
            Debug.Log("Tag: " + tagName + " has been added.");

            tagManager.ApplyModifiedProperties();

            return true;

        }

        return false;

    }

    public static string NewTag(string name) {

        CheckTag(name);

        if (name == null || name == "")
            name = "Untagged";

        return name;

    }

    public static bool RemoveTag(string tagName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        if (PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName)) {

            SerializedProperty sp;

            for (int i = 0, j = tagsProp.arraySize; i < j; i++) {

                sp = tagsProp.GetArrayElementAtIndex(i);

                if (sp.stringValue == tagName) {

                    tagsProp.DeleteArrayElementAtIndex(i);
                    Debug.Log("Tag: " + tagName + " has been removed.");
                    tagManager.ApplyModifiedProperties();
                    return true;

                }

            }

        }

        return false;

    }

    public static bool TagExists(string tagName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        return PropertyExists(tagsProp, 0, 10000, tagName);

    }

    public static bool CheckLayer(string layerName) {

        if (LayerExists(layerName))
            return true;

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        if (!PropertyExists(layersProp, 0, 31, layerName)) {

            SerializedProperty sp;

            for (int i = 8, j = 31; i < j; i++) {

                sp = layersProp.GetArrayElementAtIndex(i);

                if (sp.stringValue == "") {

                    sp.stringValue = layerName;
                    Debug.Log("Layer: " + layerName + " has been added.");
                    tagManager.ApplyModifiedProperties();
                    return true;

                }

                if (i == j)
                    Debug.Log("All allowed layers have been filled.");

            }

        }

        return false;

    }

    public static string NewLayer(string name) {

        if (name != null || name != "")
            CheckLayer(name);

        return name;

    }

    public static bool RemoveLayer(string layerName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        if (PropertyExists(layersProp, 0, layersProp.arraySize, layerName)) {

            SerializedProperty sp;

            for (int i = 0, j = layersProp.arraySize; i < j; i++) {

                sp = layersProp.GetArrayElementAtIndex(i);

                if (sp.stringValue == layerName) {

                    sp.stringValue = "";
                    Debug.Log("Layer: " + layerName + " has been removed.");
                    // Save settings
                    tagManager.ApplyModifiedProperties();
                    return true;

                }

            }

        }

        return false;

    }

    public static bool LayerExists(string layerName) {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        return PropertyExists(layersProp, 0, 31, layerName);

    }

    private static bool PropertyExists(SerializedProperty property, int start, int end, string value) {

        for (int i = start; i < end; i++) {

            SerializedProperty t = property.GetArrayElementAtIndex(i);

            if (t.stringValue.Equals(value))
                return true;

        }

        return false;

    }

}
