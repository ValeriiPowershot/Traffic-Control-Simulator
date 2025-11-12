//----------------------------------------------
//        Realistic Traffic Controller
//
// Copyright � 2014 - 2024 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Realistic_Traffic_Controller.Scripts;
using UnityEngine;
using UnityEditor;

public class RTC_EditorWindows : EditorWindow {

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Edit Settings", false, -1000)]
    public static void Opensettings() {

        Selection.activeObject = RTC_Settings.Instance;

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Add Traffic Controller To Vehicle", false, -500)]
    public static void AddRTC() {

        AddRTCCarController(Selection.activeGameObject);

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Add Traffic Controller To Vehicle", true, -500)]
    public static bool CheckAddRTC() {

        if (Selection.gameObjects.Length > 1)
            return false;

        return true;

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Create Scene Manager")]
    public static void CreateSceneManager() {

        Selection.activeGameObject = RTC.CreateSceneManager();

    }

    [MenuItem("GameObject/BoneCracker Games/RTC Scene Manager", false)]
    public static void CreateSceneManager2() {

        Selection.activeGameObject = RTC.CreateSceneManager();

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Create Traffic Spawner")]
    public static void CreateSpawner() {

        RTC_TrafficSpawner spawner = FindObjectOfType<RTC_TrafficSpawner>(true);

        if (spawner) {

            Selection.activeGameObject = spawner.gameObject;
            return;

        }

        Selection.activeGameObject = Instantiate(RTC_Settings.Instance.trafficSpawner.gameObject, Vector3.zero, Quaternion.identity);
        Selection.activeGameObject.name = RTC_Settings.Instance.trafficSpawner.name;

    }

    [MenuItem("GameObject/BoneCracker Games/RTC Traffic Spawner", false)]
    public static void CreateSpawner2() {

        RTC_TrafficSpawner spawner = FindObjectOfType<RTC_TrafficSpawner>(true);

        if (spawner) {

            Selection.activeGameObject = spawner.gameObject;
            return;

        }

        Selection.activeGameObject = Instantiate(RTC_Settings.Instance.trafficSpawner.gameObject, Vector3.zero, Quaternion.identity);
        Selection.activeGameObject.name = RTC_Settings.Instance.trafficSpawner.name;

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Create Demo Main Camera")]
    public static void CreateDemoCamera() {

        RTC_DemoCamera demoCamera = FindObjectOfType<RTC_DemoCamera>(true);

        if (demoCamera) {

            Selection.activeGameObject = demoCamera.gameObject;
            return;

        }

        Selection.activeGameObject = Instantiate(RTC_Settings.Instance.demoCamera.gameObject, Vector3.zero, Quaternion.identity);
        Selection.activeGameObject.name = RTC_Settings.Instance.demoCamera.name;

    }

    [MenuItem("GameObject/BoneCracker Games/RTC Demo Camera", false)]
    public static void CreateDemoCamera2() {

        RTC_DemoCamera demoCamera = FindObjectOfType<RTC_DemoCamera>(true);

        if (demoCamera) {

            Selection.activeGameObject = demoCamera.gameObject;
            return;

        }

        Selection.activeGameObject = Instantiate(RTC_Settings.Instance.demoCamera.gameObject, Vector3.zero, Quaternion.identity);
        Selection.activeGameObject.name = RTC_Settings.Instance.demoCamera.name;

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/Create Demo UI Canvas")]
    public static void CreateUICanvas() {

        RTC_Demo demoUICanvas = FindObjectOfType<RTC_Demo>(true);

        if (demoUICanvas) {

            Selection.activeGameObject = demoUICanvas.gameObject;
            return;

        }

        Selection.activeGameObject = Instantiate(RTC_Settings.Instance.demoUICanvas.gameObject, Vector3.zero, Quaternion.identity);
        Selection.activeGameObject.name = RTC_Settings.Instance.demoUICanvas.name;

    }

    [MenuItem("GameObject/BoneCracker Games/RTC Demo UI Canvas", false)]
    public static void CreateUICanvas2() {

        RTC_Demo demoUICanvas = FindObjectOfType<RTC_Demo>(true);

        if (demoUICanvas) {

            Selection.activeGameObject = demoUICanvas.gameObject;
            return;

        }

        Selection.activeGameObject = Instantiate(RTC_Settings.Instance.demoUICanvas.gameObject, Vector3.zero, Quaternion.identity);
        Selection.activeGameObject.name = RTC_Settings.Instance.demoUICanvas.name;

    }

    public static void AddRTCCarController(GameObject vehicleModel) {

        if (!vehicleModel.GetComponentInParent<RTC_CarController>()) {

            bool isPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(vehicleModel);

            if (isPrefab) {

                bool isModelPrefab = PrefabUtility.IsPartOfModelPrefab(vehicleModel);
                bool unpackPrefab = EditorUtility.DisplayDialog("Unpack Prefab", "This gameobject is connected to a " + (isModelPrefab ? "model" : "") + " prefab. Would you like to unpack the prefab completely? If you don't unpack it, you won't be able to move, reorder, or delete any children instance of the prefab.", "Unpack", "Don't Unpack");

                if (unpackPrefab)
                    PrefabUtility.UnpackPrefabInstance(vehicleModel, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            }

            GameObject pivot = new GameObject(vehicleModel.name);
            pivot.transform.position = RTC_GetBounds.GetBoundsCenter(vehicleModel.transform);
            pivot.transform.rotation = vehicleModel.transform.rotation;

            pivot.AddComponent<RTC_CarController>();

            vehicleModel.transform.SetParent(pivot.transform);
            Selection.activeGameObject = pivot;
            pivot.GetComponent<RTC_CarController>().CalculateBounds();

        } else {

            EditorUtility.DisplayDialog("Your Gameobject Already Has Realistic Traffic Controller", "Your Gameobject Already Has Realistic Traffic Controller", "Close");
            Selection.activeGameObject = vehicleModel;

        }

    }

    [MenuItem("Tools/BoneCracker Games/Realistic Traffic Controller/URP/Convert All Demo Materials To URP", false, 10000)]
    public static void URP() {

        EditorUtility.DisplayDialog("Converting All Demo Materials To URP", "All demo materials will be selected in your project now. After that, you'll need to convert them to URP shaders while they have been selected. You can convert them from the Edit --> Render Pipeline --> Universal Render Pipeline --> Convert Selected Materials.", "Close");

        UnityEngine.Object[] objects = new UnityEngine.Object[RTC_DemoMaterials.Instance.demoMaterials.Length];

        for (int i = 0; i < objects.Length; i++) {
            objects[i] = RTC_DemoMaterials.Instance.demoMaterials[i];
        }

        Selection.objects = objects;

    }

}
