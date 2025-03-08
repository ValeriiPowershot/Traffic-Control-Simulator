using BaseCode.Logic.ScriptableObject;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor.Vehicle
{
    [CustomEditor(typeof(VehicleScriptableObject))]
    public class VehicleScriptableObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            VehicleScriptableObject vehicle = (VehicleScriptableObject)target;

            EditorGUI.BeginChangeCheck();
            vehicle.vehiclePrefab = (GameObject)EditorGUILayout.
                ObjectField("Vehicle Prefab", vehicle.vehiclePrefab, typeof(GameObject), false);
            vehicle.minSpeed = EditorGUILayout.FloatField("Min Speed", vehicle.minSpeed);
            vehicle.maxSpeed = EditorGUILayout.FloatField("Max Speed", vehicle.maxSpeed);
            vehicle.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", vehicle.rotationSpeed);
            vehicle.rayLenght = EditorGUILayout.FloatField("Ray Length", vehicle.rayLenght);
            
            EditorGUILayout.Space();
            
            vehicle.acceptableWaitingTime = EditorGUILayout.FloatField("Acceptable Waiting Time In Second", vehicle.acceptableWaitingTime);
            vehicle.successPoints = EditorGUILayout.FloatField("Success Points", vehicle.successPoints);
            vehicle.failPoints = EditorGUILayout.FloatField("Fail Points", vehicle.failPoints);
            
            EditorGUILayout.Space();
            ShowSpeedValues(vehicle);

            EditorUtility.SetDirty(vehicle);    
        }

        private void ShowSpeedValues(VehicleScriptableObject vehicle)
        {
            float speedStep = (vehicle.maxSpeed - vehicle.minSpeed) / 3;

            float minSlowdown = vehicle.minSpeed;
            float maxSlowdown = vehicle.minSpeed + speedStep;

            float minDefault = vehicle.minSpeed + speedStep;
            float maxDefault = vehicle.minSpeed + speedStep * 2;

            float minAcceleration = vehicle.minSpeed + speedStep * 2;
            float maxAcceleration = vehicle.maxSpeed;

            EditorGUILayout.LabelField("Slowdown Speed", $"{minSlowdown:F2} - {maxSlowdown:F2}");
            EditorGUILayout.LabelField("Default Speed", $"{minDefault:F2} - {maxDefault:F2}");
            EditorGUILayout.LabelField("Acceleration Speed", $"{minAcceleration:F2} - {maxAcceleration:F2}");
        }
    }

    
    

}
