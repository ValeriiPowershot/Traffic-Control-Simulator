using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Roads
{
    public class RoadBase : MonoBehaviour
    {
        public LayerMask pointMask;
        public List<Transform> path = new List<Transform>();

        public List<Transform> onLeftPathPoints; // Example for path points on the left
        public List<Transform> onRightPathPoints;

        public float rayDistance = 2f; // Adjust based on how far roads can be
        public float pointDistance = 2f; // Adjust based on how far roads can be

        public Transform startPoint;
        public Transform endPoint;
        
        public virtual void ConnectPath(RoadBase nextBase) 
        {
            path.Clear();

            if (nextBase == null && startPoint != null) // end of the path
            {
                if (startPoint == onLeftPathPoints[0] || startPoint == onLeftPathPoints[1])
                {
                    if (startPoint == onLeftPathPoints[0])
                    {
                        path.AddRange(onLeftPathPoints);
                    }
                    else
                    {
                        path.Add(onLeftPathPoints[1]);
                        path.Add(onLeftPathPoints[0]);
                    }                    
                }
                else
                {
                    if (startPoint == onRightPathPoints[0])
                    {
                        path.AddRange(onRightPathPoints);
                    }
                    else
                    {
                        path.Add(onRightPathPoints[1]);
                        path.Add(onRightPathPoints[0]);
                    }
                }
                endPoint = path[^1];
            }
            else
            {

                if (startPoint == null) // start path
                {
                    var direction = (nextBase.transform.position - transform.position).normalized;
                    
                    if (direction == transform.forward)
                    {
                        var distanceLa = Vector3.Distance(onLeftPathPoints[0].position, nextBase.transform.position);
                        var distanceLb = Vector3.Distance(onLeftPathPoints[1].position, nextBase.transform.position);
                        var useLa = distanceLa > distanceLb;

                        if (useLa)
                        {
                            startPoint = onLeftPathPoints[0];
                            endPoint = onLeftPathPoints[1];
                        }
                        else
                        {
                            startPoint = onLeftPathPoints[1];
                            endPoint = onLeftPathPoints[0];
                        }
                        
                        path.Add(startPoint);
                        path.Add(endPoint);
                        

                        FindNextPointByRay(endPoint, nextBase);
                    }
                    else
                    {
                        var distanceLa = Vector3.Distance(onRightPathPoints[0].position, nextBase.transform.position);
                        var distanceLb = Vector3.Distance(onRightPathPoints[1].position, nextBase.transform.position);
                        var useLa = distanceLa > distanceLb;

                        if (useLa)
                        {
                            startPoint = onRightPathPoints[0];
                            endPoint = onRightPathPoints[1];
                        }
                        else
                        {
                            startPoint = onRightPathPoints[1];
                            endPoint = onRightPathPoints[0];
                        }
 
     
                        path.Add(startPoint);
                        path.Add(endPoint);

                        FindNextPointByRay(endPoint, nextBase);
    
                    }
                }
                else // middle path
                {
                    if (startPoint == onLeftPathPoints[0] || startPoint == onLeftPathPoints[1])
                    {
                        if (startPoint == onLeftPathPoints[0])
                        {
                            path.AddRange(onLeftPathPoints);
                            
                            endPoint = onLeftPathPoints[1];
                            
                            FindNextPointByRay(endPoint,nextBase);
                        }
                        else
                        {
                            path.Add(onLeftPathPoints[1]);
                            path.Add(onLeftPathPoints[0]);
                            
                            endPoint = onLeftPathPoints[0];
                            
                            FindNextPointByRay(endPoint,nextBase);
                        }                    
                    }
                    else
                    {
                        if (startPoint == onRightPathPoints[0])
                        {
                            path.AddRange(onRightPathPoints);
                            
                            endPoint = onRightPathPoints[1];
                            
                            FindNextPointByRay(endPoint,nextBase);
                        }
                        else
                        {
                            path.Add(onRightPathPoints[1]);
                            path.Add(onRightPathPoints[0]);
                            
                            endPoint = onRightPathPoints[0];
                            FindNextPointByRay(endPoint,nextBase);
                        }
                    }
                }
            }
            
        }

        public virtual void FindNextPointByRay(Transform onLeftPathPoint, RoadBase nextBase)
        {
            var boxCollider = onLeftPathPoint.GetComponent<BoxCollider>();
            boxCollider.enabled = false;
            
            var ray = new Ray(onLeftPathPoint.position, onLeftPathPoint.forward);
            if (Physics.Raycast(ray, out var hit, pointDistance, pointMask))
            {
                nextBase.startPoint = hit.transform;
            }

            boxCollider.enabled = true;
        }

        
        
        public virtual void OnDrawGizmos()
        {
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
            
            Gizmos.color = Color.red;
            foreach (var points in path)
            {
                Gizmos.DrawSphere(points.position,0.5f);
            }

            DrawArrowDirection();

        }

        public virtual void DrawArrowDirection()
        {
            Vector3 startPosition = onLeftPathPoints[0].position;
            Vector3 startPosition2 = onRightPathPoints[0].position;

            Vector3 leftDirection = transform.forward; // Left direction relative to the object
            Vector3 rightDirection = -transform.forward; // Right direction relative to the object

            float arrowLength = 2f;

            Handles.color = Color.green;
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
