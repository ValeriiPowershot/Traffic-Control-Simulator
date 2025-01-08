using System.IO;
using System.Linq;
using BaseCode.Logic.ScoringSystem;
using BaseCode.Logic.Vehicles;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor.CarCreation
{
    public class VehiclePrefabCreator : EditorWindow
    {
        private GameObject _carModelPrefab;
        private GameObject _arrow;
        private GameObject _frontRayStartPoint;
        private GameObject _indicatorOfScore;
        private GameObject _scoringMaterials;
        private GameObject _turnIndicators;
    
        private int _acceptableWaitingTime;
        private int _successPoints;
        private int _failPoints;
        private int _timeToWorstScore;
    
        private string _prefabSavePath = "Assets/Prefabs/Vehicles";
        private string _prefabSaveName;

        [MenuItem("Window/Vehicle Prefab Creator")]
        public static void ShowWindow() =>
            GetWindow<VehiclePrefabCreator>("Vehicle Prefab Creator");

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Vehicle Prefab Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Section 1
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 1: Base Vehicle Model", EditorStyles.boldLabel);
            _carModelPrefab = (GameObject)EditorGUILayout.ObjectField("Car Model Prefab", _carModelPrefab, typeof(GameObject), false);
            GUILayout.Label("The main prefab that will serve as the base for your vehicle.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Section 2
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 2: Add Additional Components", EditorStyles.boldLabel);
            _arrow = (GameObject)EditorGUILayout.ObjectField("Arrow Prefab", _arrow, typeof(GameObject), false);
            _frontRayStartPoint = (GameObject)EditorGUILayout.ObjectField("Front Ray Start Point", _frontRayStartPoint, typeof(GameObject), false);
            _indicatorOfScore = (GameObject)EditorGUILayout.ObjectField("Indicator of Score", _indicatorOfScore, typeof(GameObject), false);
            _scoringMaterials = (GameObject)EditorGUILayout.ObjectField("Scoring Materials", _scoringMaterials, typeof(GameObject), false);
            _turnIndicators = (GameObject)EditorGUILayout.ObjectField("Turn Indicators", _turnIndicators, typeof(GameObject), false);
            GUILayout.Label("Optional components to enhance your vehicle prefab.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        
            // Section 3
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 3: Set Score Values", EditorStyles.boldLabel);
            _acceptableWaitingTime = EditorGUILayout.IntField("Acceptable Waiting Time", _acceptableWaitingTime);
            _successPoints = EditorGUILayout.IntField("Success Points", _successPoints);
            _failPoints = EditorGUILayout.IntField("Fail Points", _failPoints);
            _timeToWorstScore = EditorGUILayout.IntField("Time To Worst Score", _timeToWorstScore);
            GUILayout.Label("Enter four integer values to customize your vehicle.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
        
            // Section 4
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 4: Path Settings", EditorStyles.boldLabel);
            _prefabSavePath = EditorGUILayout.TextField("Prefab Save Path", _prefabSavePath);
            GUILayout.Label("Path where the prefab will be saved. Default: 'Assets/Prefabs/Vehicles'.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);
            
            // Section 5
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 5: Name Settings", EditorStyles.boldLabel);
            _prefabSaveName = EditorGUILayout.TextField("Prefab Save Name", _prefabSaveName);
            GUILayout.Label("The name of the prefab will be called. Default: 'vehicleObject.name + .prefab.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);

            // CreateButton
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Create Vehicle Prefab", GUILayout.Width(250), GUILayout.Height(40))) 
                CreateVehiclePrefab();
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void CreateVehiclePrefab()
        {
            if (_carModelPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Car Model Prefab is required! Please assign it before creating the prefab.", "OK");
                return;
            }
            
            GameObject vehicleObject = ParentObjectCreating();

            AddComponentsToVehicle(vehicleObject);
            
            AdditionalObjectCreating(vehicleObject);
            
            SaveAsPrefab(vehicleObject);

            DestroyImmediate(vehicleObject);

            EditorUtility.DisplayDialog("Success", "Vehicle prefab created and saved to: " + _prefabSavePath, "OK");
        }
        
        private GameObject ParentObjectCreating()
        {
            GameObject vehicleObject = Instantiate(_carModelPrefab);
            vehicleObject.name = _carModelPrefab.name;
            return vehicleObject;
        }
        
        private void AdditionalObjectCreating(GameObject vehicleObject)
        {
            if (_arrow != null)
            {
                GameObject arrow = Instantiate(_arrow, vehicleObject.transform);
                arrow.name = "Arrow";
            }

            if (_frontRayStartPoint != null)
            {
                GameObject rayStartPoint = Instantiate(_frontRayStartPoint, vehicleObject.transform);
                rayStartPoint.name = "FrontRayStartPoint";
            }

            if (_indicatorOfScore != null)
            {
                GameObject scoreIndicator = Instantiate(_indicatorOfScore, vehicleObject.transform);
                scoreIndicator.name = "IndicatorOfScore";
            }

            if (_turnIndicators != null)
            {
                GameObject turnIndicators = Instantiate(_turnIndicators, vehicleObject.transform);
                turnIndicators.name = "TurnIndicators";
            }

            if (_scoringMaterials != null)
            {
                GameObject scoringObj = Instantiate(_scoringMaterials, vehicleObject.transform);
                scoringObj.name = "ScoringMaterials";
            }
        }

        private void AddComponentsToVehicle(GameObject vehicleObject)
        {
            if (vehicleObject.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rigidBody = vehicleObject.AddComponent<Rigidbody>();
                rigidBody.mass = 1;
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
            }

            if (vehicleObject.GetComponent<BoxCollider>() == null)
            {
                BoxCollider boxCollider = vehicleObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                boxCollider.size = new Vector3(2f, 1f, 5f);
            }
        
            if (vehicleObject.GetComponent<Vehicle>() == null)
            {
                Vehicle vehicle = vehicleObject.AddComponent<Vehicle>();
                vehicle.TurnLight = _turnIndicators;
                vehicle.LeftTurn = FindChildByName(_turnIndicators.transform, "LeftTurn");
                vehicle.RightTurn = FindChildByName(_turnIndicators.transform, "RightTurn");
                vehicle.RayStartPoint = _frontRayStartPoint.transform;
                vehicle.ArrowIndicatorEndPoint = _arrow.transform;
            }
        
            if (vehicleObject.GetComponent<ScoreObjectCar>() == null)
            {
                ScoreObjectCar scoreObjectCar = vehicleObject.AddComponent<ScoreObjectCar>();
            
                scoreObjectCar.AcceptableWaitingTime = _acceptableWaitingTime;
                scoreObjectCar.SuccessPoints = _successPoints;
                scoreObjectCar.FailPoints = _failPoints;
                scoreObjectCar.TimeToWorstScore = _timeToWorstScore;
            
                scoreObjectCar.IndicatorOfScore = _indicatorOfScore.GetComponent<MeshRenderer>();
                scoreObjectCar.ScoreMaterialsComponent = _scoringMaterials.GetComponent<ScoringMaterials>();
            }
        }

        private Transform FindChildByName(Transform parent, string objectName) =>
            parent.Cast<Transform>().FirstOrDefault(child => child.name == objectName);

        private void SaveAsPrefab(GameObject vehicleObject)
        {
            if (!Directory.Exists(_prefabSavePath)) 
                Directory.CreateDirectory(_prefabSavePath);

            string filePath = _prefabSaveName == null 
                ? System.IO.Path.Combine(_prefabSavePath, vehicleObject.name + ".prefab") 
                : System.IO.Path.Combine(_prefabSavePath, _prefabSaveName + ".prefab");
            
            PrefabUtility.SaveAsPrefabAsset(vehicleObject, filePath);
        }
    }
}
