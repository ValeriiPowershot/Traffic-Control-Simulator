using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private int _speed;
        private int _acceleration;
        private int _slowdown;
        private float _rayLenght = 2f;

        private bool _needToCopyColliderSize;

        private string _prefabSavePath = "Assets/Prefabs/Vehicles";
        private readonly string _soSavePath = "Assets/SoObjects/Vehicles"; 
        private string _prefabSaveName;

        private PreviewRenderUtility _previewRenderUtility;
        private float _distance = 30f; // Camera distance from origin
        private Vector2 _orbitAngles = new Vector2(90, 8); // (Yaw, Pitch)
        private Vector2 _previousMousePos;
        private bool _isDragging = false;
        private Bounds _combinedBounds = default;

        private List<Type> _carTypes = new List<Type>();
        private int _selectedTypeIndex = 0;
        private string[] _carTypeNames;
        
        [MenuItem("Window/Vehicle Prefab Creator")]
        public static void ShowWindow()
        {
            var window = GetWindow<VehiclePrefabCreator>("Vehicle Prefab Creator");
            window.minSize = new Vector2(600, 1000); // Minimum width and height
        }
        private void OnEnable()
        {
            _carTypes = Assembly.GetAssembly(typeof(BasicCar)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BasicCar)) && !t.IsAbstract).ToList();
            _carTypes.Add(typeof(BasicCar));
            _carTypeNames = _carTypes.Select(t => t.Name).ToArray();
            
            foreach (var typeName in _carTypeNames)
            {
                Debug.Log(typeName);
            }

            _previewRenderUtility = new PreviewRenderUtility(true)
            {
                cameraFieldOfView = 30f
            };
            LoadAllResources();
        }

        private void OnDisable()
        {
            _previewRenderUtility.Cleanup();
        }
        private void OnGUI()
        {

            GUILayout.Space(10);
            
            GUILayout.Label("Vehicle Prefab Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            RenderCreateButton();
            GUILayout.Space(10);

            RenderScoreValuesSection();
            GUILayout.Space(20);

            RenderVehicleParametersSection();
            GUILayout.Space(20);

            RenderPathSettingsSection();
            GUILayout.Space(20);

            RenderNameSettingsSection();
            GUILayout.Space(20);

            RenderBaseVehicleModelSection();
            
            EditorGUILayout.Space();

            SelectCarType();

        }

        private void SelectCarType()
        {
            EditorGUILayout.LabelField("Select Car Type", EditorStyles.boldLabel);
            _selectedTypeIndex = EditorGUILayout.Popup("Car Type", _selectedTypeIndex, _carTypeNames);
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
            
            if (_vehicleModelPrefab != null)
            {
                _distance = EditorGUILayout.Slider("Distance", _distance, 1, 40);
                Draw3DPreview();
            }
            
            EditorGUILayout.EndVertical();
        }

        private void Draw3DPreview()
        {
            Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(true));

            HandleMouseInput(previewRect);

            Vector3 cameraPosition = Quaternion.Euler(_orbitAngles.y, _orbitAngles.x, 0) * new Vector3(0, 0, -_distance);

            _previewRenderUtility.BeginPreview(previewRect, GUIStyle.none);
            _previewRenderUtility.camera.transform.position = cameraPosition;
            _previewRenderUtility.camera.transform.LookAt(Vector3.zero);
            _previewRenderUtility.camera.nearClipPlane = 0.1f;
            _previewRenderUtility.camera.farClipPlane = 1000f;
            _previewRenderUtility.lights[0].transform.rotation = Quaternion.Euler(50, 50, 0);
            _vehicleModelPrefab.transform.position = Vector3.zero;
            
            RenderPrefab(_vehicleModelPrefab.transform);
            
            Vector3 frontPosition = GetFrontPosition(_vehicleModelPrefab.transform);
            _frontRayStartPoint.transform.position = frontPosition; 
            RenderPrefab(_frontRayStartPoint.transform);
            DrawDebugRayMesh(frontPosition, frontPosition + _frontRayStartPoint.transform.forward * _rayLenght, Color.red);
            
            _previewRenderUtility.Render();
            
            Texture previewTexture = _previewRenderUtility.EndPreview();
            GUI.DrawTexture(previewRect, previewTexture, ScaleMode.StretchToFill, false);
        }
 
        private Vector3 GetFrontPosition(Transform vehicleObject)
        {
            if(_combinedBounds == default)
                _combinedBounds = GetBound(vehicleObject);
            
            Vector3 frontOffset = new Vector3(0, 0, _combinedBounds.extents.z);
            return _combinedBounds.center + vehicleObject.transform.rotation * frontOffset;
        }
        private Vector3 GetBackPosition(Transform vehicleObject)
        {
            if(_combinedBounds == default)
                _combinedBounds = GetBound(vehicleObject);
            
            Vector3 frontOffset = new Vector3(0, 0, _combinedBounds.extents.z);
            return _combinedBounds.center - vehicleObject.transform.rotation * frontOffset;
        }
        private Vector3 GetUpperMidPosition(Transform vehicleObject)
        {
            if (_combinedBounds == default)
                _combinedBounds = GetBound(vehicleObject);

            Vector3 upperMidOffset = new Vector3(0, _combinedBounds.extents.y, 0);
            return _combinedBounds.center + vehicleObject.transform.rotation * upperMidOffset;
        }


        private Bounds GetBound(Transform vehicleObject)
        {
            MeshRenderer[] meshRenderers = vehicleObject.GetComponentsInChildren<MeshRenderer>();

            if (meshRenderers.Length > 0)
            {
                _combinedBounds = meshRenderers[0].bounds;

                foreach (MeshRenderer renderer in meshRenderers)
                {
                    _combinedBounds.Encapsulate(renderer.bounds);
                }

                return _combinedBounds;
            }
            Debug.LogError("No MeshRenderers found on the 'vehicleObject' or its children.");

            return default;
        }

        private void DrawDebugRayMesh(Vector3 start, Vector3 end, Color color)
        {
            Mesh lineMesh = new Mesh();

            float lineWidth = 0.05f;

            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up) * lineWidth;

            Vector3[] vertices = new Vector3[4];
            vertices[0] = start - perpendicular;
            vertices[1] = start + perpendicular;
            vertices[2] = end + perpendicular;
            vertices[3] = end - perpendicular;

            int[] triangles = { 0, 1, 2, 2, 3, 0 }; // Two triangles forming a quad
            lineMesh.vertices = vertices;
            lineMesh.triangles = triangles;

            Material lineMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = color
            };
            _previewRenderUtility.DrawMesh(lineMesh, Vector3.zero, Quaternion.identity, lineMaterial, 0);
        }

        private void HandleMouseInput(Rect previewRect)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && previewRect.Contains(e.mousePosition) && e.button == 0)
            {
                _isDragging = true;
                _previousMousePos = e.mousePosition;
                e.Use();
            }
            else if (e.type == EventType.MouseUp && e.button == 0)
            {
                _isDragging = false;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && _isDragging)
            {
                Vector2 delta = e.mousePosition - _previousMousePos;
                _orbitAngles.x += delta.x * 0.3f;  
                _orbitAngles.y += delta.y * 0.3f; 

                _orbitAngles.y = Mathf.Clamp(_orbitAngles.y, -90f, 90f);

                _previousMousePos = e.mousePosition;
                e.Use();
            }
        }
        
        private void RenderPrefab(Transform prefabTransform, Vector3 offset = default)
        {
            foreach (MeshFilter meshFilter in prefabTransform.GetComponentsInChildren<MeshFilter>())
            {
                MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshFilter.sharedMesh != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    Material[] materials = meshRenderer.sharedMaterials;

                    // Apply the offset to move the object
                    Vector3 position = meshFilter.transform.position + offset;

                    for (int i = 0; i < mesh.subMeshCount; i++)
                    {
                        Material material = (i < materials.Length) ? materials[i] : null;

                        _previewRenderUtility.DrawMesh(
                            mesh,
                            position,
                            meshFilter.transform.rotation,
                            material,
                            i
                        );
                    }
                }
            }
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
            
            _speed = EditorGUILayout.IntField("Vehicle Speed", _speed);
            _acceleration = EditorGUILayout.IntField("Vehicle Acceleration", _acceleration);
            _slowdown = EditorGUILayout.IntField("Slowdown", _slowdown);
            _rayLenght = EditorGUILayout.FloatField("Ray Length", _rayLenght);

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

        private void ChangeVehicleLayer(GameObject vehicleObject) => vehicleObject.layer = LayerMask.NameToLayer("Car");

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
            vehicleObject.TryGetComponent(out BoxCollider vehicleCollider);

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
            _scoringMaterials = Resources.Load<GameObject>($"ForVehicleCreator/ReactionIndicator");
            _turnIndicators = Resources.Load<GameObject>($"ForVehicleCreator/TurnIndicators");
             
            if (_arrow == null || _frontRayStartPoint == null || _scoringMaterials == null || _turnIndicators == null)
                Debug.Log("Path has no game object!");
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
            var frontPos = GetFrontPosition(vehicleObject.transform);
            var backPos = GetBackPosition(vehicleObject.transform);
            var upperMid = GetUpperMidPosition(vehicleObject.transform);
            
            CreateAndAttachChild(_arrow, vehicleObject, "Arrow", Quaternion.identity, upperMid + Vector3.up*2);
            CreateAndAttachChild(_frontRayStartPoint, vehicleObject, "FrontRayStartPoint", position:frontPos);
            CreateAndAttachChild(_turnIndicators, vehicleObject, "TurnIndicators", position: backPos);
            CreateAndAttachChild(_scoringMaterials, vehicleObject, "ScoringMaterials", position: upperMid + Vector3.up*2);
        }

        private static void CreateAndAttachChild(GameObject prefab, GameObject parent, string name, Quaternion? rotation = null, Vector3? position = null)
        {
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab,parent.transform);
                instance.name = name;

                if (rotation.HasValue) 
                    instance.transform.rotation = rotation.Value;
                if (position.HasValue)
                    instance.transform.position = position.Value;
            }
        }

        private void AddComponentsToVehicle(GameObject vehicleObject)
        {
            if(!vehicleObject.TryGetComponent(out Rigidbody rigidBody)) 
                rigidBody = vehicleObject.AddComponent<Rigidbody>();

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
                
                boxCollider.size = _combinedBounds.size;
                boxCollider.center = _combinedBounds.center - vehicleObject.transform.position;
            }
            
            if (vehicleObject.GetComponent<BasicCar>() == null)
            {
                Type selectedCarType = _carTypes[_selectedTypeIndex];
                BasicCar basicCar = (BasicCar)vehicleObject.AddComponent(selectedCarType);
                
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
            string path = _soSavePath;

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
            vehicleScriptableObject.RayLenght = _rayLenght;

            AssetDatabase.CreateAsset(vehicleScriptableObject, soFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"ScriptableObject created and saved at {soFilePath}");
        }
        
        
    }
}
