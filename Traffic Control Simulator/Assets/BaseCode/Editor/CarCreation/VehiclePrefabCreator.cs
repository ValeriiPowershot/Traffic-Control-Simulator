using System.IO;
using BaseCode.Infrastructure;
using BaseCode.Logic.ScoringSystem;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Editor.CarCreation
{
    public class VehiclePrefabCreator : EditorWindow
    {
        private GameObject _vehicleModelPrefab;
        private GameObject _arrow;
        private GameObject _frontRayStartPoint;
        private GameObject _scoringMaterials;
        private GameObject _turnIndicators;

        private int _acceptableWaitingTime;
        private int _successPoints;
        private int _failPoints;
        private int _timeToWorstScore;

        private int _rotationSpeed;
        private int _speed;
        private int _acceleration;
        private int _slowdown;
        private float _rayLenght;
        private int _indexPath;

        private bool _needToCopyColliderSize;

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

            RenderBaseVehicleModelSection();
            GUILayout.Space(10);

            RenderScoreValuesSection();
            GUILayout.Space(20);

            RenderVehicleParametersSection();
            GUILayout.Space(20);

            RenderPathSettingsSection();
            GUILayout.Space(20);

            RenderNameSettingsSection();
            GUILayout.Space(20);

            RenderCreateButton();
        }

        
        //Section 1
        private void RenderBaseVehicleModelSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 1: Base Vehicle Model", EditorStyles.boldLabel);

            _vehicleModelPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Car Model Prefab",
                _vehicleModelPrefab,
                typeof(GameObject),
                false
            );

            GUILayout.Label(
                "The main prefab that will serve as the base for your vehicle (it is desirable to use a prefab that already stores a collider in it).",
                EditorStyles.wordWrappedLabel
            );

            EditorGUILayout.EndVertical();
        }

        //Section 2
        private void RenderScoreValuesSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 2: Set Score Values", EditorStyles.boldLabel);

            _acceptableWaitingTime = EditorGUILayout.IntSlider("Acceptable Waiting Time", _acceptableWaitingTime, 0, 35);
            _successPoints = EditorGUILayout.IntSlider("Success Points", _successPoints, 0, 15);
            _failPoints = EditorGUILayout.IntSlider("Fail Points", _failPoints, 0, 40);
            _timeToWorstScore = EditorGUILayout.IntSlider("Time To Worst Score", _timeToWorstScore, 0, 20);

            _needToCopyColliderSize = EditorGUILayout.Toggle("Copy Collider Parameters", _needToCopyColliderSize);

            GUILayout.Label("Enter four integer values to customize your vehicle.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
        }

        //Section 3
        private void RenderVehicleParametersSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 3: Set Vehicle Parameters", EditorStyles.boldLabel);

            _rotationSpeed = EditorGUILayout.IntField("Rotation Speed", _rotationSpeed);
            _speed = EditorGUILayout.IntField("Vehicle Speed", _speed);
            _acceleration = EditorGUILayout.IntField("Vehicle Acceleration", _acceleration);
            _slowdown = EditorGUILayout.IntField("Slowdown", _slowdown);
            _rayLenght = EditorGUILayout.FloatField("Ray Length", _rayLenght);
            _indexPath = EditorGUILayout.IntField("Index Path", _indexPath);

            GUILayout.Label("Enter four integer values to customize your vehicle.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndVertical();
        }

        //Section 4
        private void RenderPathSettingsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 4: Path Settings", EditorStyles.boldLabel);

            _prefabSavePath = EditorGUILayout.TextField("Prefab Save Path", _prefabSavePath);

            GUILayout.Label(
                "Path where the prefab will be saved. Default: 'Assets/Prefabs/Vehicles'.",
                EditorStyles.wordWrappedLabel
            );

            EditorGUILayout.EndVertical();
        }

        //Section 5
        private void RenderNameSettingsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Step 5: Name Settings", EditorStyles.boldLabel);

            _prefabSaveName = EditorGUILayout.TextField("Prefab Save Name", _prefabSaveName);

            GUILayout.Label(
                "The name of the prefab will be called. Default: 'vehicleObject.name + .prefab'.",
                EditorStyles.wordWrappedLabel
            );

            EditorGUILayout.EndVertical();
        }

        private void RenderCreateButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create Vehicle Prefab", GUILayout.Width(250), GUILayout.Height(40)))
            {
                CreateVehiclePrefab();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void CreateVehiclePrefab()
        {
            if (_vehicleModelPrefab == null)
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    "Car Model Prefab is required! Please assign it before creating the prefab.",
                    "OK"
                );
                return;
            }

            GameObject vehicleObject = ParentObjectCreating();
            LoadAllResources();
            AddComponentsToVehicle(vehicleObject);
            AdditionalObjectCreating(vehicleObject);
            CheckVehicleCollider(vehicleObject);
            DeleteWheelsColliders();
            ChangeVehicleLayer(vehicleObject);
            SaveAsPrefab(vehicleObject);

            DestroyImmediate(vehicleObject);

            EditorUtility.DisplayDialog(
                "Success",
                "Vehicle prefab created and saved to: " + _prefabSavePath,
                "OK"
            );
        }

        private void ChangeVehicleLayer(GameObject vehicleObject) =>
            vehicleObject.layer = LayerMask.NameToLayer("Car");

        private void DeleteWheelsColliders()
        {
            GameObject wheelCollidersHolder = ObjectFinder.FindObjectInParent(
                _vehicleModelPrefab,
                "Colliders"
            );

            if (wheelCollidersHolder == null) return;
            
            DestroyImmediate(wheelCollidersHolder);
        }

        private void CheckVehicleCollider(GameObject vehicleObject)
        {
            if (!_needToCopyColliderSize || ObjectFinder.FindObjectInParent(_vehicleModelPrefab, "Body").GetComponent<BoxCollider>() == null) return;

            BoxCollider vehicleCollider = ObjectFinder.FindObjectInParent(_vehicleModelPrefab, "Body").GetComponent<BoxCollider>();

            BoxCollider objectCollider = vehicleObject.GetComponent<BoxCollider>();
            objectCollider.size = vehicleCollider.size;
            objectCollider.center = vehicleCollider.center;
            objectCollider.isTrigger = true;

            vehicleCollider.isTrigger = true;
        }

        private void LoadAllResources()
        {
            _arrow = Resources.Load<GameObject>($"ForVehicleCreator/Arrow");
            _frontRayStartPoint = Resources.Load<GameObject>($"ForVehicleCreator/FrontRayStartPoint");
            _scoringMaterials = Resources.Load<GameObject>($"ForVehicleCreator/ScoringMaterials");
            _turnIndicators = Resources.Load<GameObject>($"ForVehicleCreator/TurnIndicators");
        }

        private GameObject ParentObjectCreating()
        {
            GameObject vehicleObject = Instantiate(_vehicleModelPrefab);
            vehicleObject.name = _vehicleModelPrefab.name;
            vehicleObject.layer = Layers.Car;
            return vehicleObject;
        }

        private void AdditionalObjectCreating(GameObject vehicleObject)
        {
            CreateAndAttachChild(_arrow, vehicleObject, "Arrow", Quaternion.identity);
            CreateAndAttachChild(_frontRayStartPoint, vehicleObject, "FrontRayStartPoint");
            CreateAndAttachChild(_turnIndicators, vehicleObject, "TurnIndicators");
            CreateAndAttachChild(_scoringMaterials, vehicleObject, "ScoringMaterials");
        }

        private static void CreateAndAttachChild(GameObject prefab, GameObject parent, string name, Quaternion? rotation = null)
        {
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, parent.transform);
                instance.name = name;

                if (rotation.HasValue) 
                    instance.transform.rotation = rotation.Value;
            }
        }

        private void AddComponentsToVehicle(GameObject vehicleObject)
        {
            Rigidbody rigidBody;
            if(!vehicleObject.TryGetComponent<Rigidbody>(out rigidBody))
            {
                rigidBody = vehicleObject.AddComponent<Rigidbody>();
            }

            rigidBody.mass = 1;
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;

            if (!vehicleObject.TryGetComponent(out BoxCollider boxCollider))
            {
                boxCollider = vehicleObject.AddComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    Debug.LogError("Failed to add BoxCollider.");
                    return;
                }
            }

            if (vehicleObject.GetComponent<BasicCar>() == null)
            {
                BasicCar basicCar = vehicleObject.AddComponent<BasicCar>();
                basicCar.TurnLight = _turnIndicators;
                basicCar.LeftTurn = ObjectFinder.FindObjectInParent(_turnIndicators, "LeftTurn").transform;
                basicCar.RightTurn = ObjectFinder.FindObjectInParent(_turnIndicators, "RightTurn").transform;
                basicCar.RayStartPoint = _frontRayStartPoint.transform;
                basicCar.ArrowIndicatorEndPoint = _arrow.transform;
            }

            if (vehicleObject.GetComponent<ScoreObjectCar>() == null)
            {
                ScoreObjectCar scoreObjectCar = vehicleObject.AddComponent<ScoreObjectCar>();

                scoreObjectCar.AcceptableWaitingTime = _acceptableWaitingTime;
                scoreObjectCar.SuccessPoints = _successPoints;
                scoreObjectCar.FailPoints = _failPoints;
                scoreObjectCar.TimeToWorstScore = _timeToWorstScore;

                scoreObjectCar.ScoreMaterialsComponent = _scoringMaterials.GetComponent<ScoringMaterials>();
            }
        }

        private void SaveAsPrefab(GameObject vehicleObject)
        {
            if (!Directory.Exists(_prefabSavePath)) 
                Directory.CreateDirectory(_prefabSavePath);

            string prefabPath = string.IsNullOrEmpty(_prefabSaveName)
                ? System.IO.Path.Combine(_prefabSavePath, vehicleObject.name + ".prefab")
                : System.IO.Path.Combine(_prefabSavePath, _prefabSaveName + ".prefab");

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(vehicleObject, prefabPath);
            CreateVehicleScriptableObject(prefab);
        }

        private void CreateVehicleScriptableObject(GameObject vehiclePrefab)
        {
            const string path = "Assets/ScriptableObjects/Vehicles";

            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);

            string fileName = string.IsNullOrEmpty(_prefabSaveName)
                ? vehiclePrefab.name + ".asset"
                : _prefabSaveName + ".asset";

            string soFilePath = System.IO.Path.Combine(path, fileName);

            VehicleScriptableObject vehicleScriptableObject = CreateInstance<VehicleScriptableObject>();
            vehicleScriptableObject.VehiclePrefab = vehiclePrefab;

            vehicleScriptableObject.DefaultSpeed = _speed;
            vehicleScriptableObject.AccelerationSpeed = _acceleration;
            vehicleScriptableObject.SlowdownSpeed = _slowdown;
            vehicleScriptableObject.RotationSpeed = _rotationSpeed;
            vehicleScriptableObject.RayLenght = _rayLenght;
            vehicleScriptableObject.IndexPath = _indexPath;

            AssetDatabase.CreateAsset(vehicleScriptableObject, soFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"ScriptableObject created and saved at {soFilePath}");
        }
    }
}
