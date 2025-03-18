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
            
            EditorGUILayout.LabelField("Vehicle Prefab");
            ShowCarConfig(vehicle);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Time Config");
            ShowMaxAcceptableWaitingTime(vehicle);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Success Config");
            ShowMaxSuccessPoint(vehicle);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Speed Config");
            ShowSpeedValues(vehicle);

            EditorUtility.SetDirty(vehicle);    
        }

        private void ShowCarConfig(VehicleScriptableObject vehicle)
        {
            vehicle.vehiclePrefab = (GameObject)EditorGUILayout.
                ObjectField("Vehicle Prefab", vehicle.vehiclePrefab, typeof(GameObject), false);
            vehicle.minSpeed = EditorGUILayout.FloatField("Min Speed", vehicle.minSpeed);
            vehicle.maxSpeed = EditorGUILayout.FloatField("Max Speed", vehicle.maxSpeed);
            vehicle.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", vehicle.rotationSpeed);
            vehicle.rayLenght = EditorGUILayout.FloatField("Ray Length", vehicle.rayLenght);
        }

        private void ShowMaxAcceptableWaitingTime(VehicleScriptableObject vehicle)
        {
            vehicle.maxAcceptableWaitingTime = EditorGUILayout.FloatField("Min AcceptableWaitingTime", vehicle.maxAcceptableWaitingTime);
            vehicle.minAcceptableWaitingTime = EditorGUILayout.FloatField("Max AcceptableWaitingTime", vehicle.minAcceptableWaitingTime);
        }
        private void ShowMaxSuccessPoint(VehicleScriptableObject vehicle)
        {
            vehicle.maxSuccessPoints = EditorGUILayout.FloatField("Min SuccessPoints", vehicle.maxSuccessPoints);
            vehicle.minSuccessPoints = EditorGUILayout.FloatField("Max SuccessPoints", vehicle.minSuccessPoints);
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
