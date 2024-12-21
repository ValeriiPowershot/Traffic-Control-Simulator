using System.Collections.Generic;
using System.Linq;
using BaseCode.Logic.Lights;
using UnityEngine;

namespace Script.Roads
{
    public class TripleRoadIntersection : RoadBase
    {
        [Header("Triple Road Intersection")]
        public RoadBase forwardRoad; // 0 , 0 , 1

        public List<Transform> onForwardRoad;
        
        public Transform forwardRayPoint; // Position for left ray

        public List<BasicLight> basicLights;

        public List<Transform> onForwardPathA;
        public List<Transform> onForwardPathB;
        
        public List<Transform> onLeftPathA;
        public List<Transform> onLeftPathB;

        public List<Transform> onRightPathA;
        public List<Transform> onRightPathB;

        public List<Transform> onRightPathC;
        
        
        public override void ConnectPath(RoadBase nextBase)
        {
            if (startPoint == null)
            {
                Debug.Log($" {name} start is null");
                return;
            }
            path.Clear();
            
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
            
            // get its end
            endPoint = path[^1];
            Debug.Log("R"); 
            
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
        
 

        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            if (forwardRayPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(forwardRayPoint.position, forwardRayPoint.forward * rayDistance);
            }
        }

        public override void DrawArrowDirection()
        {
            
        }

    }
}