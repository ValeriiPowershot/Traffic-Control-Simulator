using Gley.TrafficSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    [CustomEditor(typeof(TrailerComponent))]
    public class TrailerComponentEditor : UnityEditor.Editor
    {
        const string carHolderName = "TrailerHolder";
        const string wheelsHolderName = "Wheels";

        private TrailerComponent targetScript;

        private void OnEnable()
        {
            targetScript = (TrailerComponent)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Automatically Computed", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Width: " + targetScript.width.ToString());
            EditorGUILayout.LabelField("Height: " + targetScript.height.ToString());
            EditorGUILayout.LabelField("Length: " + targetScript.length.ToString());

            EditorGUILayout.Space();
            if (GUILayout.Button("Configure Trailer"))
            {
                ConfigureTrailer((TrailerComponent)target);
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("View Tutorial"))
            {
                Application.OpenURL("https://youtu.be/moGHcd2Jaa4");
            }
        }


        public static void ConfigureTrailer(TrailerComponent targetScript)
        {
            bool correct = true;
            SetCarHolder(targetScript, ref correct);
            SetPivotAtAnchor(targetScript, ref correct);
            SetupRigidbody(targetScript, ref correct);
            AssignWheels(targetScript, ref correct);
            SetWheelDimensions(targetScript, ref correct);
            SetTrailerDimensions(targetScript, ref correct);
            CheckColliers(targetScript, ref correct);
            CreateJoint(targetScript, ref correct);

            if (correct)
            {
                Debug.Log("Success! All references for " + targetScript.name + " ware correct");
                EditorUtility.SetDirty(targetScript);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError(targetScript.name + " will not work correctly. See above messages for details");
            }
        }


        private static void SetTrailerDimensions(TrailerComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;

            MeshFilter[] allMeshes = targetScript.trailerHolder.GetComponentsInChildren<MeshFilter>();
            if (allMeshes.Length == 0)
            {
                LogError(ref correct, "No meshes found inside " + targetScript.name);
                return;
            }

            for (int i = 0; i < allMeshes.Length; i++)
            {
                float xSize = allMeshes[i].sharedMesh.bounds.size.x * allMeshes[i].transform.localScale.x;
                float ySize = allMeshes[i].sharedMesh.bounds.size.x * allMeshes[i].transform.localScale.x;
                float zSize = allMeshes[i].sharedMesh.bounds.size.z * allMeshes[i].transform.localScale.z;

                if (targetScript.length < zSize)
                {
                    targetScript.length = zSize;
                }

                if (targetScript.height < ySize)
                {
                    targetScript.height = ySize;
                }

                if (targetScript.width < xSize)
                {
                    targetScript.width = xSize;
                }
            }
        }


        private static void LogError(ref bool correct, string message)
        {
            correct = false;
            Debug.LogError(message + " Auto assign will stop now.");
            Debug.Log("Please fix the above errors and press Configure Trailer again or assign missing references and values manually.");
        }


        private static void SetCarHolder(TrailerComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;
            Transform trailerHolder = targetScript.transform.Find(carHolderName);
            if (trailerHolder == null)
            {
                if (targetScript.trailerHolder != null)
                {
                    Debug.Log("A GameObject was manually assigned inside " + nameof(targetScript.trailerHolder) + ". Make sure is the root GameObject");
                }
                else
                {
                    LogError(ref correct, "A GameObject named " + carHolderName + " was not found under " + targetScript.name);
                }
            }
            else
            {
                targetScript.trailerHolder = trailerHolder;
            }
        }


        private static void SetPivotAtAnchor(TrailerComponent targetScript, ref bool correct)
        {
            if (targetScript.truckConnectionPoint == null)
            {
                LogError(ref correct, "Truck connection point not set. Please assign the truck connection transform");
                return;
            }

            targetScript.trailerHolder.localPosition = new Vector3(0, 0, -targetScript.truckConnectionPoint.localPosition.z);
        }


        private static void SetupRigidbody(TrailerComponent targetScript, ref bool correct)
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
            }
        }


        private static void AssignWheels(TrailerComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;
            Transform wheelsHolder = targetScript.trailerHolder.Find(wheelsHolderName);
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

                targetScript.allWheels[i].wheelPosition = Wheel.WheelPosition.Other;

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


        private static void SetWheelDimensions(TrailerComponent targetScript, ref bool correct)
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


        private static void CheckColliers(TrailerComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;

            Collider[] allColliders = targetScript.GetComponentsInChildren<Collider>();
            bool hasColliders = false;
            float colliderHeight = 0;
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (!allColliders[i].isTrigger)
                {
                    float colliderDimension = allColliders[i].bounds.size.y * allColliders[i].transform.lossyScale.y;
                    if (colliderHeight < colliderDimension)
                    {
                        colliderHeight = colliderDimension;
                    }
                    hasColliders = true;
                }
            }

            if (!hasColliders)
            {
                LogError(ref correct, "No collider found -> Please assign a collider on the trailer.");
                return;
            }

            if (colliderHeight == 0)
            {
                LogError(ref correct, "Collider height is 0 -> Verify your colliders.");
            }
        }


        private static void CreateJoint(TrailerComponent targetScript, ref bool correct)
        {
            if (!correct)
                return;

            ConfigurableJoint joint = targetScript.joint;

            if(joint==null)
            {
                joint = targetScript.gameObject.AddComponent<ConfigurableJoint>();
            }

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
            targetScript.joint = joint;
        }
    }
}