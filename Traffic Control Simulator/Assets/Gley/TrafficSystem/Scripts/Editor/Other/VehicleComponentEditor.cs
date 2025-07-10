using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    [CustomEditor(typeof(VehicleComponent))]
    public class VehicleComponentEditor : UnityEditor.Editor
    {
        const string carHolderName = "CarHolder";
        const string frontTriggerHolderName = "FrontTriggerHolder";
        const string frontTriggerName = "FrontTrigger";
        const string wheelsHolderName = "Wheels";
        const string shadowHolderName = "ShadowHolder";
        const string maxTriggerLengthName = "Max Trigger Length";
        const string updateTriggerName = "Update Trigger";
        const string trailerComponentName = "Trailer Component";
        const string trailerAnchorPointName = "Trailer Connection Point";

        private VehicleComponent targetScript;


        private void OnEnable()
        {
            targetScript = (VehicleComponent)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            targetScript.updateTrigger = EditorGUILayout.Toggle(new GUIContent(updateTriggerName, "It checked, vehicle trigger length will increase with speed"), targetScript.updateTrigger);
            if (targetScript.updateTrigger)
            {
                targetScript.maxTriggerLength = EditorGUILayout.FloatField(new GUIContent(maxTriggerLengthName, "The length of the front trigger at max speed"), targetScript.maxTriggerLength);
            }

            targetScript.trailer = (TrailerComponent)EditorGUILayout.ObjectField(trailerComponentName, targetScript.trailer, typeof(TrailerComponent), true);
            if(targetScript.trailer)
            {
                targetScript.trailerConnectionPoint = (Transform)EditorGUILayout.ObjectField(trailerAnchorPointName, targetScript.trailerConnectionPoint, typeof(Transform), true);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Automatically Computed", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Length: " + targetScript.length.ToString());
            EditorGUILayout.LabelField("Collider Height: " + targetScript.coliderHeight.ToString());
            EditorGUILayout.LabelField("Wheel Distance: " + targetScript.wheelDistance.ToString());
            EditorGUILayout.LabelField("Excluded: " + targetScript.excluded.ToString());

            if (EditorGUI.EndChangeCheck())
            {
                Save(targetScript);
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Configure Car"))
            {
                ConfigureCar(targetScript);
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("View Tutorial"))
            {
                Application.OpenURL("https://youtu.be/moGHcd2Jaa4");
            }
        }


        public static void ConfigureCar(VehicleComponent targetScript)
        {
            bool correct = true;
            SetCarHolder(targetScript, ref correct);
            SetupRigidbody(targetScript, ref correct);
            AssignWheels(targetScript, ref correct);
            CreateFrontTrigger(targetScript, ref correct);
            AddShadow(targetScript);
            SetPivotOnBackWheels(targetScript, ref correct);
            SetWheelDimensions(targetScript, ref correct);
            CheckForZero(targetScript.accelerationTime, nameof(targetScript.accelerationTime), ref correct);
            CheckForZero(targetScript.minPossibleSpeed, nameof(targetScript.minPossibleSpeed), ref correct);
            CheckForZero(targetScript.maxPossibleSpeed, nameof(targetScript.maxPossibleSpeed), ref correct);
            AssignLights(targetScript);
            AssignVisibilityScript(targetScript);
            CheckColliers(targetScript, ref correct);
            ConnectTrailer(targetScript, ref correct);

            if (correct)
            {
                Debug.Log("Success! All references for " + targetScript.name + " ware correct");
                Save(targetScript);
            }
            else
            {
                Debug.LogError(targetScript.name + " will not work correctly. See above messages for details");
            }
        }


        private static void ConnectTrailer(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;

            if (targetScript.trailer == null)
                return;

            if(targetScript.trailerConnectionPoint==null)
            {
                LogError(ref correct, "Trailer Connection Point is not assigned. " +
                    "Please assign it on Vehicle Component.");
                    return;
            }

            targetScript.trailer.transform.position = new Vector3(targetScript.trailerConnectionPoint.position.x,targetScript.carHolder.position.y, targetScript.trailerConnectionPoint.position.z);
        
            if(targetScript.trailer.joint==null)
            {
                LogError(ref correct, "No joint assigned. Please Configure Trailer first.");
                return;
            }

            if(targetScript.trailer.truckConnectionPoint==null)
            {
                LogError(ref correct, "No truckConnectionPoint assigned. Please Configure Trailer first.");
                return;
            }

            targetScript.trailer.joint.connectedBody = targetScript.rb;
            targetScript.trailer.joint.autoConfigureConnectedAnchor = false;
            targetScript.trailer.joint.anchor = targetScript.trailer.truckConnectionPoint.GetLocalPositionRelativeToTopParent(targetScript.trailer.transform);
            targetScript.trailer.joint.connectedAnchor = targetScript.trailerConnectionPoint.GetLocalPositionRelativeToTopParent(targetScript.transform);
            targetScript.trailer.rb.mass = targetScript.rb.mass;
            targetScript.trailer.rb.drag = targetScript.rb.drag;
            targetScript.trailer.rb.angularDrag = targetScript.rb.angularDrag;
        }


        static void Save(VehicleComponent targetScript)
        {
            EditorUtility.SetDirty(targetScript);
            AssetDatabase.SaveAssets();
        }


        private static void CheckColliers(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;

            Collider[] allColliders = targetScript.carHolder.GetComponentsInChildren<Collider>();
            targetScript.coliderHeight = 0;
            bool hasColliders = false;
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (!allColliders[i].isTrigger)
                {
                    float colliderDimension = allColliders[i].bounds.size.y * allColliders[i].transform.lossyScale.y;
                    if (targetScript.coliderHeight < colliderDimension)
                    {
                        targetScript.coliderHeight = colliderDimension;
                    }
                    hasColliders = true;
                }
            }

            if (!hasColliders)
            {
                LogError(ref correct, "No collider found -> Please assign a collider on the car.");
            }

            if (targetScript.coliderHeight == 0)
            {
                LogError(ref correct, "Collider height is 0 -> Verify your colliders or press configure car in prefab mode.");
            }
        }


        private static void AssignVisibilityScript(VehicleComponent targetScript)
        {
            VisibilityScript[] allComponents = targetScript.gameObject.GetComponentsInChildren<VisibilityScript>();
            if (allComponents.Length > 1)
            {
                for (int i = 1; i < allComponents.Length; i++)
                {
                    DestroyImmediate(allComponents[i]);
                }
            }
            if (allComponents.Length == 1)
            {
                if (allComponents[0].GetComponent<Renderer>() != null)
                {
                    targetScript.visibilityScript = allComponents[0];
                    return;
                }
                else
                {
                    DestroyImmediate(allComponents[0]);
                }
            }

            Renderer renderer = targetScript.GetComponentInChildren<Renderer>();
            targetScript.visibilityScript = renderer.gameObject.AddComponent<VisibilityScript>();
        }


        private static void AssignLights(VehicleComponent targetScript)
        {
            VehicleLightsComponent vehicleLights = targetScript.GetComponent<VehicleLightsComponent>();
            if (vehicleLights)
            {
                AssignGameobject(targetScript, ref vehicleLights.frontLights, "FrontLights");
                AssignGameobject(targetScript, ref vehicleLights.reverseLights, "ReverseLights");
                AssignGameobject(targetScript, ref vehicleLights.rearLights, "RearLights");
                AssignGameobject(targetScript, ref vehicleLights.stopLights, "StopLights");
                AssignGameobject(targetScript, ref vehicleLights.blinkerLeft, "BlinkersLeft");
                AssignGameobject(targetScript, ref vehicleLights.blinkerRight, "BlinkersRight");
            }
        }


        private static void AssignGameobject(VehicleComponent targetScript, ref GameObject objToAssign, string objectName)
        {
            Transform go = targetScript.transform.FindDeepChild(objectName);
            if (go == null)
            {
                Debug.Log(objectName + " not found");
                return;
            }
            objToAssign = go.gameObject;
        }


        private static void SetWheelDimensions(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;
            for (int i = 0; i < targetScript.allWheels.Length; i++)
            {
                MeshFilter[] allMeshes = targetScript.allWheels[i].wheelTransform.GetComponentsInChildren<MeshFilter>();
                if (allMeshes.Length == 0)
                {
                    Debug.LogWarning("No meshes found inside " + targetScript.allWheels[0].wheelTransform);
                }

                float maxSize = 0;
                float wheelRadius = 0;
                //create mesh
                for (int j = 0; j < allMeshes.Length; j++)
                {
                    float xSize = allMeshes[j].sharedMesh.bounds.size.x * allMeshes[j].transform.lossyScale.x;
                    float ySize = allMeshes[j].sharedMesh.bounds.size.y * allMeshes[j].transform.lossyScale.y;
                    float zSize = allMeshes[j].sharedMesh.bounds.size.z * allMeshes[j].transform.lossyScale.z;
                    bool changed = false;
                    if (xSize > maxSize)
                    {
                        maxSize = xSize;
                        changed = true;
                    }
                    if (ySize > maxSize)
                    {
                        maxSize = ySize;
                        changed = true;
                    }
                    if (zSize > maxSize)
                    {
                        maxSize = zSize;
                        changed = true;
                    }
                    if (changed)
                    {
                        wheelRadius = Mathf.Max(xSize, ySize, zSize);
                        wheelRadius /= 2;
                    }
                }
                if (targetScript.allWheels[i].wheelRadius == 0)
                {
                    targetScript.allWheels[i].wheelRadius = wheelRadius;
                }
                if (targetScript.maxSuspension == 0)
                {
                    targetScript.allWheels[i].maxSuspension = wheelRadius / 2;
                }
                else
                {
                    targetScript.allWheels[i].maxSuspension = targetScript.maxSuspension;
                }

                targetScript.allWheels[i].raycastLength = targetScript.allWheels[i].wheelRadius + targetScript.allWheels[i].maxSuspension;
                targetScript.allWheels[i].wheelCircumference = targetScript.allWheels[i].wheelRadius * Mathf.PI * 2;
            }
        }


        private static void CheckForZero(float value, string propertyName, ref bool correct)
        {
            if (value <= 0)
            {
                LogError(ref correct, propertyName + " needs to be > 0");
            }
        }


        private static void SetPivotOnBackWheels(VehicleComponent targetScript, ref bool correct)
        {
            Vector3 poz = new Vector3();
            int nr = 0;
            for (int i = 0; i < targetScript.allWheels.Length; i++)
            {
                if (targetScript.allWheels[i].wheelPosition == Wheel.WheelPosition.Back)
                {
                    nr++;
                    poz += targetScript.allWheels[i].wheelTransform.localPosition;
                }
            }
            if (nr == 0)
            {
                LogError(ref correct, "No back wheels set. Mark Wheel Position as Back for the back car wheels");
                return;
            }

            targetScript.carHolder.localPosition = new Vector3(0, 0, -(poz / nr).z);
        }


        private static void AssignWheels(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;
            Transform wheelsHolder = targetScript.carHolder.Find(wheelsHolderName);
            if (wheelsHolder == null)
            {
                for (int i = 0; i < targetScript.allWheels.Length; i++)
                {
                    if (targetScript.allWheels[i].wheelTransform == null)
                    {
                        LogError(ref correct, "A GameObject named " + wheelsHolderName + " was not found under " + targetScript.name);
                        return;
                    }
                }
                Debug.Log("All wheels ware manually assigned, make sure they are correct");
                return;
            }

            //verify if wheels ware already assigned
            bool allAssigned = true;
            if (targetScript.allWheels != null)
            {
                if (targetScript.allWheels.Length < 2)
                {
                    allAssigned = false;
                }
                for (int i = 0; i < targetScript.allWheels.Length; i++)
                {
                    if (targetScript.allWheels[i].wheelTransform == null)
                    {
                        allAssigned = false;
                    }
                }
            }
            else
            {
                allAssigned = false;
            }

            if (allAssigned == true)
            {
                Debug.Log("All wheels ware manually assigned, make sure they are correct");
                return;
            }

            if (wheelsHolder.childCount == 0)
            {
                LogError(ref correct, "No GameObject was not found under " + wheelsHolderName);
            }

            targetScript.allWheels = new Wheel[wheelsHolder.childCount];
            for (int i = 0; i < wheelsHolder.childCount; i++)
            {
                targetScript.allWheels[i] = new Wheel();
                targetScript.allWheels[i].wheelTransform = wheelsHolder.GetChild(i);
                if (targetScript.allWheels[i].wheelTransform.name.Contains("Front"))
                {
                    targetScript.allWheels[i].wheelPosition = Wheel.WheelPosition.Front;
                }
                if (targetScript.allWheels[i].wheelTransform.name.Contains("Back") || targetScript.allWheels[i].wheelTransform.name.Contains("Rear"))
                {
                    targetScript.allWheels[i].wheelPosition = Wheel.WheelPosition.Back;
                }
                try
                {
                    targetScript.allWheels[i].wheelGraphics = wheelsHolder.GetChild(i).GetChild(0);
                }
                catch
                {
                    LogError(ref correct, "No GameObject was not found under " + wheelsHolder.GetChild(i).name);
                }
            }
        }


        private static void AddShadow(VehicleComponent targetScript)
        {
            Transform shadowHolder = targetScript.carHolder.transform.Find(shadowHolderName);
            if (shadowHolder)
            {
                targetScript.shadowHolder = shadowHolder;
            }
            else
            {
                Debug.Log("Shadow not found. Please assign the shadow manually if you need it");
            }
        }


        private static void CreateFrontTrigger(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;
            float triggerLength = targetScript.distanceToStop + targetScript.triggerLength;
            float triggerHeight = 1;

            Transform frontTriggerHolder = targetScript.carHolder.transform.Find(frontTriggerHolderName);
            GleyPrefabUtilities.DestroyTransform(frontTriggerHolder);
            if (frontTriggerHolder == null)
            {
                if (GleyPrefabUtilities.EditingInsidePrefab())
                {
                    GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                    frontTriggerHolder = new GameObject(frontTriggerHolderName).transform;
                    frontTriggerHolder.SetParent(prefabRoot.transform.Find(targetScript.carHolder.name).transform, false);

                }
                else
                {
                    frontTriggerHolder = new GameObject(frontTriggerHolderName).transform;
                    frontTriggerHolder.SetParent(targetScript.carHolder.transform, false);
                }
            }

            Vector3 triggerPoz = new Vector3();
            int nr = 0;
            for (int i = 0; i < targetScript.allWheels.Length; i++)
            {
                if (targetScript.allWheels[i].wheelPosition == Wheel.WheelPosition.Front)
                {
                    nr++;
                    triggerPoz += targetScript.allWheels[i].wheelTransform.position;
                }
            }

            if (nr == 0)
            {
                LogError(ref correct, "No front wheels set. Mark Wheel Position as Front for the front car wheels");
                return;
            }

            triggerPoz = triggerPoz / nr;
            frontTriggerHolder.position = triggerPoz;
            targetScript.frontTrigger = frontTriggerHolder;

            targetScript.wheelDistance = targetScript.transform.InverseTransformPoint(triggerPoz).z;
            if (targetScript.wheelDistance == 0)
            {
                LogError(ref correct, " Distance between wheels is 0. Make sure your wheel assignments are correct");
                return;
            }

            Transform frontTrigger = frontTriggerHolder.Find(frontTriggerName);
            if (frontTrigger == null)
            {
                MeshFilter[] allMeshes = targetScript.carHolder.transform.GetComponentsInChildren<MeshFilter>();
                if (allMeshes.Length == 0)
                {
                    LogError(ref correct, "No meshes found inside " + targetScript.name);
                    return;
                }

                float maxSize = 0;
                float triggerSize = 0;
                //create mesh
                for (int i = 0; i < allMeshes.Length; i++)
                {
                    float xSize = allMeshes[i].sharedMesh.bounds.size.x * allMeshes[i].transform.localScale.x;
                    float ySize = allMeshes[i].sharedMesh.bounds.size.y * allMeshes[i].transform.localScale.y;
                    float zSize = allMeshes[i].sharedMesh.bounds.size.z * allMeshes[i].transform.localScale.z;
                    bool changed = false;
                    if (xSize > maxSize)
                    {
                        maxSize = xSize;
                        changed = true;
                    }
                    if (zSize > maxSize)
                    {
                        maxSize = zSize;
                        changed = true;
                    }
                    if (changed)
                    {
                        triggerSize = Mathf.Min(xSize, zSize);
                        targetScript.length = Mathf.Max(xSize, zSize);
                    }

                    if (ySize > triggerHeight)
                    {
                        triggerHeight = ySize;
                    }

                }
                frontTrigger = new GameObject(frontTriggerName).transform;
                frontTrigger.SetParent(frontTriggerHolder);
                BoxCollider boxCollider = frontTrigger.gameObject.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(triggerSize, triggerHeight, triggerLength);
                boxCollider.center = new Vector3(0, 0, triggerLength / 2);
                boxCollider.isTrigger = true;
                frontTriggerHolder.gameObject.SetLayer(targetScript.gameObject.layer);
                frontTrigger.transform.localPosition = new Vector3(0, triggerHeight / 2, 0);
            }
        }


        private static void SetupRigidbody(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;

            Rigidbody rb = targetScript.GetComponent<Rigidbody>();
            if (rb == null)
            {
                LogError(ref correct, "RigidBody not found on " + targetScript.name);
            }
            else
            {
                targetScript.rb = rb;
                if (rb.drag == 0)
                {
                    targetScript.rb.drag = 0.1f;
                    targetScript.rb.angularDrag = 3;
                }
            }
        }


        private static void SetCarHolder(VehicleComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;
            Transform carHolder = targetScript.transform.Find(carHolderName);
            if (carHolder == null)
            {
                if (targetScript.carHolder != null)
                {
                    Debug.Log("A GameObject was manually assigned inside " + nameof(targetScript.carHolder) + ". Make sure is the root GameObject");
                }
                else
                {
                    LogError(ref correct, "A GameObject named " + carHolderName + " was not found under " + targetScript.name);
                }
            }
            else
            {
                targetScript.carHolder = carHolder;
            }
        }


        private static void LogError(ref bool correct, string message)
        {
            correct = false;
            Debug.LogError(message + " Auto assign will stop now.");
            Debug.Log("Please fix the above errors and press Configure Car again or assign missing references and values manually.");
        }
    }
}