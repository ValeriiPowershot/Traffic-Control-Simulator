using System.Collections.Generic;
using System.Linq;
using BaseCode.Logic.ScriptableObject;
using UnityEditor;
using UnityEngine;

namespace BaseCode.Logic.Roads
{
    public class RoadBase : MonoBehaviour
    {
        public RoadsScriptableObject roadSo;
        
        public Mesh roadMesh;
        public LayerMask pointMask;
        public List<Transform> path = new List<Transform>();
        public List<Transform> decelerationPoints = new List<Transform>();
        public List<Transform> accelerationPoints = new List<Transform>();

        public List<Transform> onLeftPathPoints; // Example for path points on the left
        public List<Transform> onRightPathPoints;

        public float pointDistance = 2f; // Adjust based on how far roads can be

        public Transform startPoint;
        public Transform endPoint;

        public virtual void ConnectPath(RoadBase nextBase)
        {
            path.Clear(); 
            decelerationPoints.Clear();
            accelerationPoints.Clear();

            if (IsPathEnd(nextBase)) // end of the path
            {
                if (IsStartPointIsSame(onLeftPathPoints))
                {
                    if (startPoint == onLeftPathPoints[0])
                        AddPathPoint(onLeftPathPoints);
                    else
                        AddReversedPathPoints(onLeftPathPoints);
                }
                else
                {
                    if (startPoint == onRightPathPoints[0])
                        AddPathPoint(onRightPathPoints);
                    else
                        AddReversedPathPoints(onRightPathPoints);
                }
                
                endPoint = path[^1];
            }
            else
            {
                if (startPoint == null) // start path
                {
                    Vector3 direction = (nextBase.transform.position - transform.position).normalized;

                    if (direction == transform.forward)
                    {
                        bool useLa = FindCloserStartPoint(onLeftPathPoints,nextBase.transform.position);

                        if (useLa)
                            UseNewStartEndPoints(onLeftPathPoints);
                        else
                            UseNewReversedStartEndPoints(onLeftPathPoints);

                        FindNextPointByRay(endPoint, nextBase);
                    }
                    else
                    {
                        bool useLa = FindCloserStartPoint(onRightPathPoints,nextBase.transform.position);;

                        if (useLa)
                            UseNewStartEndPoints(onRightPathPoints);
                        else
                            UseNewReversedStartEndPoints(onRightPathPoints);

                        FindNextPointByRay(endPoint, nextBase);
                    }
                }
                
                else // middle path
                {
                    if (IsStartPointIsSame(onLeftPathPoints))
                    {
                        if (startPoint == onLeftPathPoints[0])
                            UseNewStartEndPoints(onLeftPathPoints);
                        else
                            UseNewReversedStartEndPoints(onLeftPathPoints);

                        FindNextPointByRay(endPoint, nextBase);
                    }
                    else
                    {
                        if (startPoint == onRightPathPoints[0])
                            UseNewStartEndPoints(onRightPathPoints);
                        else
                            UseNewReversedStartEndPoints(onRightPathPoints);

                        FindNextPointByRay(endPoint, nextBase);
                    }
                }
            }
            
            AddAccDecPoints();
        }

        private void AddAccDecPoints()
        {
            if (path.Count <= 3) // normal road has 3 points 
                return;
            int halfPathLength = path.Count / 2;
            decelerationPoints.AddRange(path.Take(halfPathLength));
            accelerationPoints.AddRange(path.Skip(halfPathLength));
        }

        private bool FindCloserStartPoint(List<Transform> transforms, Vector3 transformPosition)
        {
            float distanceLa = Vector3.Distance(transforms[0].position, transformPosition);
            float distanceLb = Vector3.Distance(transforms[^1].position, transformPosition);
            return distanceLa > distanceLb;
        }

        private void UseNewStartEndPoints(List<Transform> points)
        {
            startPoint = points[0];
            endPoint = points[^1];
            
            path.AddRange(points);
        }
        
        private void UseNewReversedStartEndPoints(List<Transform> points)
        {
            List<Transform> reversedPoints = new List<Transform>(points);
            reversedPoints.Reverse();
            UseNewStartEndPoints(reversedPoints);
        }

        private bool IsPathEnd(RoadBase nextBase) =>
            nextBase == null && startPoint != null;

        private bool IsStartPointIsSame(List<Transform> points) =>
            startPoint == points[0] || startPoint == points[^1];

        private void AddPathPoint(List<Transform> point) =>
            path.AddRange(point);

        private void AddReversedPathPoints(List<Transform> points)
        {
            List<Transform> reversedPoints = new List<Transform>(points);
            reversedPoints.Reverse();
            path.AddRange(reversedPoints);
        }

        private void FindNextPointByRay(Transform onLeftPathPoint, RoadBase nextBase)
        {
            BoxCollider boxCollider = onLeftPathPoint.GetComponent<BoxCollider>();
            boxCollider.enabled = false;

            Ray ray = new Ray(onLeftPathPoint.position, onLeftPathPoint.forward);
            
            if (Physics.Raycast(ray, out var hit, pointDistance, pointMask)) 
                nextBase.startPoint = hit.transform;

            if (nextBase is TripleRoadIntersection) // when entering 
                decelerationPoints.Add(path[^1]);

            boxCollider.enabled = true;
        }

        public virtual void OnDrawGizmos()
        {
            if(roadSo == null || roadSo.canDrawRoadGizmo == false)
                return;
            
            if (startPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(startPoint.position, startPoint.forward * pointDistance);
            }

            if (endPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(endPoint.position, endPoint.forward * pointDistance);
            }

            Gizmos.color = Color.yellow;
            
            foreach (Transform points in accelerationPoints) 
                Gizmos.DrawSphere(points.position, 0.5f);
            
            Gizmos.color = Color.red;
            
            foreach (Transform points in decelerationPoints) 
                Gizmos.DrawSphere(points.position, 0.5f);
            
            DrawArrowDirection();
        }

        protected virtual void DrawArrowDirection()
        {
            
            Vector3 startPosition = onLeftPathPoints[0].position;
            Vector3 startPosition2 = onRightPathPoints[0].position;

            Vector3 leftDirection = transform.forward; // Left direction relative to the object
            Vector3 rightDirection = -transform.forward; // Right direction relative to the object

            float arrowLength = roadSo.arrowLength;
            Handles.color = roadSo.arrowColor;
            
            Handles.ArrowHandleCap(
                0,
                startPosition,
                Quaternion.LookRotation(leftDirection),
                arrowLength,
                EventType.Repaint
            );

            Handles.ArrowHandleCap(
                0,
                startPosition2,
                Quaternion.LookRotation(rightDirection),
                arrowLength,
                EventType.Repaint
            );
        }

    }
}
