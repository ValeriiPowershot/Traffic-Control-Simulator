using System.Collections.Generic;
using System.Linq;
using BaseCode.Logic.Entity.Npcs.Npc;
using BaseCode.Logic.Lights;
using BaseCode.Logic.Lights.Handler.Abstracts;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Roads
{
    public class TripleRoadIntersection : RoadBase
    {
        [Header("Triple Road Intersection")]
        // path
        public List<Transform> onForwardPathA;
        public List<Transform> onForwardPathB;
        
        public List<Transform> onLeftPathA;
        public List<Transform> onLeftPathB;

        public List<Transform> onRightPathA;
        public List<Transform> onRightPathB;

        // lights
        public List<BasicLight> basicLights;

        public List<MeshRenderer> meshRayRoads = new List<MeshRenderer>();
        
        public override void ConnectPath(RoadBase nextBase)
        {
            path.Clear();
            accelerationPoints.Clear();
            decelerationPoints.Clear();
            
            // find start point path
            if (startPoint == onForwardPathA[0] || startPoint == onForwardPathB[0])
            {
                var distanceB = Vector3.Distance(onForwardPathB[^1].transform.position, nextBase.onLeftPathPoints[0].transform.position); 
                var distanceA = Vector3.Distance(onForwardPathA[^1].transform.position, nextBase.onLeftPathPoints[0].transform.position);
            
                var shouldIUseA = distanceA < distanceB;
                
                path.AddRange(shouldIUseA ? onForwardPathA : onForwardPathB);
            }
            else if(startPoint == onRightPathA[0] || startPoint == onRightPathB[0])
            {
                var distanceB = Vector3.Distance(onRightPathB[^1].transform.position, nextBase.onLeftPathPoints[0].transform.position); 
                var distanceA = Vector3.Distance(onRightPathA[^1].transform.position, nextBase.onLeftPathPoints[0].transform.position);
            
                var shouldIUseA = distanceA < distanceB;
                path.AddRange(shouldIUseA ? onRightPathA : onRightPathB);
                // B
            }
            else if(startPoint == onLeftPathA[0] || startPoint == onLeftPathB[0])
            {
                var distanceA = Vector3.Distance(onLeftPathA[^1].transform.position, nextBase.onLeftPathPoints[0].transform.position); 
                var distanceB = Vector3.Distance(onLeftPathB[^1].transform.position, nextBase.onLeftPathPoints[0].transform.position);
            
                var shouldIUseA = distanceA < distanceB;
                path.AddRange(shouldIUseA ? onLeftPathA : onLeftPathB);
            }
            
            int halfPathLength = path.Count / 2;
            decelerationPoints.AddRange(path.Take(halfPathLength)); // hurt me plenty
            accelerationPoints.AddRange(path.Skip(halfPathLength));
            
            // get its end
            endPoint = path[^1];
            
            // find next start point
            var distanceL1 = Vector3.Distance(endPoint.transform.position, nextBase.onLeftPathPoints[0].transform.position); 
            var distanceL2 = Vector3.Distance(endPoint.transform.position, nextBase.onLeftPathPoints[1].transform.position);
            var useDistanceL1 = distanceL1 < distanceL2;
            
            var distanceR1 = Vector3.Distance(endPoint.transform.position, nextBase.onRightPathPoints[0].transform.position); 
            var distanceR2 = Vector3.Distance(endPoint.transform.position, nextBase.onRightPathPoints[1].transform.position);
            var useDistanceR1 = distanceR1 < distanceR2;

            if (useDistanceL1)
            {
                if (useDistanceR1)
                {
                    nextBase.startPoint = distanceL1 < distanceR1 ? nextBase.onLeftPathPoints[0] : nextBase.onRightPathPoints[0];
                }
                else
                {
                    nextBase.startPoint = distanceL1 < distanceR2 ? nextBase.onLeftPathPoints[0] : nextBase.onRightPathPoints[1];
                }
                
            }
            else
            {
                if (useDistanceR1)
                {
                    nextBase.startPoint = distanceL2 < distanceR1 ? nextBase.onLeftPathPoints[1] : nextBase.onRightPathPoints[0];
                }
                else
                {
                    nextBase.startPoint = distanceL2 < distanceR2 ? nextBase.onLeftPathPoints[1] : nextBase.onRightPathPoints[1];
                }
            }
        }
        
        
        /*public override void DrawArrowDirection()
        {
            
        }*/

        public void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<VehicleBase>(out var vehicle))
            {
                vehicle.vehicleController.VehicleLightController.ExitIntersection();
            }
        }

        public virtual bool GenerateNpc<T>(RoadsScriptableObject roadSoBase) where T : NpcBase
        {
            LightBase lightBase = GetALight();

            if (lightBase == null)
                return false;

            var npcBase = roadSoBase.GetNpc<T>();
            if (npcBase == null)
                return false;

            GameObject createdNpc = (GameObject)PrefabUtility.InstantiatePrefab(npcBase.prefab.gameObject);
            createdNpc.transform.position = lightBase.targetA.transform.position;
            

            var sceneRoadGenerationController = FindObjectOfType<SceneRoadGenerationController>();
            if (sceneRoadGenerationController != null)
                createdNpc.transform.SetParent(sceneRoadGenerationController.transform);
            
            NpcBase npcBaseComponent  = createdNpc.GetComponent<T>();   
            lightBase.controlledNpcs = npcBaseComponent;
            
            
            PrefabUtility.RecordPrefabInstancePropertyModifications(createdNpc);
            PrefabUtility.RecordPrefabInstancePropertyModifications(lightBase);

            npcBaseComponent.npcController.a = lightBase.targetA;
            npcBaseComponent.npcController.b = lightBase.targetB;
            npcBaseComponent.npcController.target = lightBase.targetA;

            lightBase.npcGroundIndicator.gameObject.SetActive(true);
            
            npcBaseComponent.npcController.npcScriptableObject = npcBase;
            
            return true;
        }
        
        public virtual bool DestroyNpc() 
        {
            LightBase lightBase = GetALight(false);
            if (lightBase == null)
                return false;
            if (lightBase.controlledNpcs == null)
                return false;

            lightBase.npcGroundIndicator.gameObject.SetActive(false);
            DestroyImmediate(lightBase.controlledNpcs.gameObject);
            return true;
        }

        public virtual LightBase GetALight(bool findNull = true)
        {
            return findNull ? 
                basicLights.FirstOrDefault(basicLight => basicLight.ControlledNpc == null) : 
                basicLights.FirstOrDefault(basicLight => basicLight.ControlledNpc != null);
        }

        public override void OnDrawGizmos()
        {
            if(roadSo == null || roadSo.canDrawRoadGizmo == false)
                return;
            
            base.OnDrawGizmos();

            var circleSize = roadSo.circleSize;
            Gizmos.color = Color.red;
            foreach (Transform points in onForwardPathA) 
                Gizmos.DrawSphere(points.position, circleSize);
            foreach (Transform points in onForwardPathB) 
                Gizmos.DrawSphere(points.position, circleSize);
            
            Gizmos.color = Color.yellow;
            foreach (Transform points in onRightPathA) 
                Gizmos.DrawSphere(points.position, circleSize);
            foreach (Transform points in onRightPathB) 
                Gizmos.DrawSphere(points.position, circleSize);
            
            Gizmos.color = Color.blue;
            foreach (Transform points in onLeftPathA) 
                Gizmos.DrawSphere(points.position, circleSize);
            foreach (Transform points in onLeftPathB) 
                Gizmos.DrawSphere(points.position, circleSize);
        }
        
        protected override void DrawArrowDirection()
        {
            
        }

        public void ToggleMeshRayRoad()
        {
            foreach (var meshRenderer in meshRayRoads)
            {
                meshRenderer.enabled = !meshRenderer.enabled;
            }
        }
    }
}