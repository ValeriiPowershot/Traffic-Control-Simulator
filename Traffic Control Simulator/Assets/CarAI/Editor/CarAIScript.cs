using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CarAIEditorScript : EditorWindow
{
    private bool setupcar = false;
    private bool spawncheckpoints = false;
    private bool createintersection = false;

    //Car setup

    private GameObject carmodel;

    private Transform frontRight;
    private Transform frontLeft;
    private Transform rearRight;
    private Transform rearLeft;

    private WheelCollider frontRightCollider;
    private WheelCollider frontLeftCollider;
    private WheelCollider rearLeftCollider;
    private WheelCollider rearRightCollider;

    private float acceleration = 10000;
    private float breaking = 100000;
    private int speedLimit;
    private Transform check;
    private int count = 0;

    //Intersection creator
    private int stops = 0;

    [MenuItem("Window/Car AI")]

    public static void ShowWindow()
    {
        GetWindow<CarAIEditorScript>("Car AI");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Setup car"))
        {
            setupcar = true;
            spawncheckpoints = false;
            createintersection = false;
        }
        else if (GUILayout.Button("Checkpoints"))
        {
            spawncheckpoints = true;
            setupcar = false;
            createintersection = false;
        }
        else if(GUILayout.Button("Create intersection"))
        {
            createintersection = true;
            setupcar = false;
            spawncheckpoints = false;
        }
        else if(GUILayout.Button("Spawn if block"))
        {
            GameObject p = new GameObject("If block");
            
            GameObject stopper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stopper.name = "Stopper";
            stopper.transform.localScale = Vector3.one;
            stopper.transform.SetParent(p.transform);
            stopper.GetComponent<BoxCollider>().isTrigger = true;
            StopScript stopScript = stopper.AddComponent<StopScript>();
            stopScript.stop = false;

            GameObject checker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            checker.name = "Checker";
            checker.transform.localScale = Vector3.one;
            checker.GetComponent<BoxCollider>().isTrigger = true;
            checker.transform.SetParent(p.transform);
            checker.transform.position = stopper.transform.forward * 2f;
            CheckerScript checkerScript = checker.AddComponent<CheckerScript>();

            checkerScript.stopScripts.Add(stopScript);
        }

        if (setupcar)
        {
            SetupCar();
        }
        else if (spawncheckpoints)
        {
            SpawnCheckpoints();
        }
        else if(createintersection)
        {
            CreateIntersection();
        }

        GUILayout.Label("", EditorStyles.boldLabel);
        GUILayout.Label("Check the file doc.pdf for documentation.", EditorStyles.boldLabel);

    }

    void SetupCar()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Car model", EditorStyles.boldLabel);
        carmodel = (GameObject)EditorGUILayout.ObjectField(carmodel, typeof(GameObject), true);

        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Front right wheel transform", EditorStyles.boldLabel);
        frontRight = (Transform)EditorGUILayout.ObjectField(frontRight, typeof(Transform), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Front left wheel transform", EditorStyles.boldLabel);
        frontLeft = (Transform)EditorGUILayout.ObjectField(frontLeft, typeof(Transform), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Rear right wheel transform", EditorStyles.boldLabel);
        rearRight = (Transform)EditorGUILayout.ObjectField(rearRight, typeof(Transform), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Rear left wheel transform", EditorStyles.boldLabel);
        rearLeft = (Transform)EditorGUILayout.ObjectField(rearLeft, typeof(Transform), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Front right wheel collider", EditorStyles.boldLabel);
        frontRightCollider = (WheelCollider)EditorGUILayout.ObjectField(frontRightCollider, typeof(WheelCollider), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Front left wheel collider", EditorStyles.boldLabel);
        frontLeftCollider = (WheelCollider)EditorGUILayout.ObjectField(frontLeftCollider, typeof(WheelCollider), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Rear right wheel collider", EditorStyles.boldLabel);
        rearRightCollider = (WheelCollider)EditorGUILayout.ObjectField(rearRightCollider, typeof(WheelCollider), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Rear left wheel collider", EditorStyles.boldLabel);
        rearLeftCollider = (WheelCollider)EditorGUILayout.ObjectField(rearLeftCollider, typeof(WheelCollider), true);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Acceleration", EditorStyles.boldLabel);
        acceleration = EditorGUILayout.FloatField(acceleration);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Breaking", EditorStyles.boldLabel);
        breaking = EditorGUILayout.FloatField(breaking);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Speed limit", EditorStyles.boldLabel);
        speedLimit = EditorGUILayout.IntField(speedLimit);

        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Check position", EditorStyles.boldLabel);
        check = (Transform)EditorGUILayout.ObjectField(check, typeof(Transform), true);

        EditorGUILayout.EndVertical();


        if (GUILayout.Button("Apply") && !carmodel.GetComponent<CarAIController>())
        {
            CarAIController controller = carmodel.AddComponent<CarAIController>();

            controller.frontRight = frontRight;
            controller.frontLeft = frontLeft;
            controller.rearLeft = rearLeft;
            controller.rearRight = rearRight;

            controller.frontRightCollider = frontRightCollider;
            controller.frontLeftCollider = frontLeftCollider;
            controller.rearRightCollider = rearRightCollider;
            controller.rearLeftCollider = rearLeftCollider;

            controller.acceleration = acceleration;
            controller.breaking = breaking;
            controller.speedLimit = speedLimit;
            if(controller.checks[0] == null)
            {
                controller.checks[0] = check; 
            }
        }
    }

    void SpawnCheckpoints()
    {   
        GUILayout.Label("Spawn checkpoint and press on it. You will find instructions there.", EditorStyles.boldLabel);

        if (GUILayout.Button("Spawn checkpoint"))
        {
            GameObject parent = new GameObject("Checkpoints");
            CheckpointScript script = new CheckpointScript();
            spawnCheckpoint(Vector3.zero, Vector3.one, parent.transform, ref script);
        }
        else if(GUILayout.Button("Connect selected checkpoints"))
        {
            GameObject[] selected = Selection.gameObjects;
            
            bool canConnect = true;

            for(int i = 0; i + 1 < selected.Length; i++)
            {
                CheckpointScript script = selected[i].GetComponent<CheckpointScript>();
                if(!script || isAlreadyConnected(script.nextCheckpoints, selected[i + 1].transform))
                {
                    canConnect = false;
                    break;
                }
            }

            if(canConnect)
            {
                for(int i = 0; i + 1 < selected.Length; i++)
                {
                    CheckpointScript script = selected[i].GetComponent<CheckpointScript>();
                    script.nextCheckpoints.Add(selected[i+1].transform);
                }
            }
        }
        else if(GUILayout.Button("Disconnect selected checkpoints"))
        {
            GameObject[] selected = Selection.gameObjects;

            for(int i = 0; i + 1 < selected.Length; i++)
            {
                CheckpointScript script = selected[i].GetComponent<CheckpointScript>();
                if(script)
                {
                    script.nextCheckpoints.Remove(selected[i+1].transform);
                }
            }
        }
        else if(GUILayout.Button("Spawn checkpoint between two selected checkpoints"))
        {
            GameObject[] selected = Selection.gameObjects;

            if(selected.Length == 2)
            {
                if(isElementInList(selected[1].GetComponent<CheckpointScript>().nextCheckpoints, selected[0].transform))
                {
                    GameObject c = selected[1];
                    selected[1] = selected[0];
                    selected[0] = c;
                }

                if(selected.Length == 2)
                {
                    CheckpointScript script0 = selected[0].GetComponent<CheckpointScript>(); 
                    CheckpointScript script1 = selected[1].GetComponent<CheckpointScript>(); 

                    if(script0 && script1)
                    {
                        CheckpointScript middleScript = new CheckpointScript();
                        GameObject checkpoint = spawnCheckpoint((selected[0].transform.position + selected[1].transform.position) / 2f, Vector3.one, selected[0].transform.parent, ref middleScript);

                        script0.nextCheckpoints.Remove(selected[1].transform);
                        script0.nextCheckpoints.Add(checkpoint.transform);
                        middleScript.nextCheckpoints.Add(selected[1].transform);
                    }
                }
            }
        }
    }

    private bool isElementInList(List<Transform> list, Transform element)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i] == element)
            {
                return true;
            }
        }

        return false;
    }

    void CreateIntersection()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Number of stops", EditorStyles.boldLabel);
        stops = EditorGUILayout.IntField(stops);

        EditorGUILayout.EndVertical();


        if(GUILayout.Button("Spawn intersection") && stops != 0)
        {
            GameObject intersection = new GameObject("Intersection");

            IntersectionScript intersectionScript = intersection.AddComponent<IntersectionScript>();

            for(int i = 1; i <= stops; i++)
            {
                GameObject stop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stop.name = "Stop" + i.ToString();
                stop.transform.SetParent(intersection.transform);
                stop.GetComponent<BoxCollider>().isTrigger = true;

                StopScript stopScript = stop.AddComponent<StopScript>();
                stopScript.stop = true;

                intersectionScript.stops.Add(stop);

            }

        }


    }

    bool isAlreadyConnected(List<Transform> nextCheckpoints, Transform checkpoint)
    {
        bool result = false;

        for(int i = 0; i < nextCheckpoints.Count; i++)
        {
            if(nextCheckpoints[i] == checkpoint)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private GameObject spawnCheckpoint(Vector3 pos, Vector3 scale, Transform parent, ref CheckpointScript script)
    {
        GameObject checkpoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        checkpoint.transform.position = pos;
        checkpoint.transform.localScale = scale;
        checkpoint.name = "Checkpoint_" + count.ToString();
        checkpoint.GetComponent<BoxCollider>().isTrigger = true;
        count++;

        if(parent != null)
        {
            checkpoint.transform.SetParent(parent);
        }

        script = checkpoint.AddComponent<CheckpointScript>();
        script.speedLimit = -1;

        return checkpoint;
    }

}
