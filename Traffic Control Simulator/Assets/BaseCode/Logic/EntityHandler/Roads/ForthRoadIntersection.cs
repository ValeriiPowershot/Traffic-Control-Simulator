using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseCode.Logic.EntityHandler.Roads
{
    public class ForthRoadIntersection : TripleRoadIntersection
    {
        public List<Transform> onForwardPathC;
        public List<Transform> onLeftPathC;
        public List<Transform> onRightPathC;

        // path
        public List<Transform> onBackwardPathA;
        public List<Transform> onBackwardPathB;
        public List<Transform> onBackwardPathC;

        public override void ConnectPath(RoadBase nextBase)
        {
            path.Clear();
            accelerationPoints.Clear();
            decelerationPoints.Clear();
            
            if (startPoint == onForwardPathA[0] || startPoint == onForwardPathB[0] || startPoint == onForwardPathC[0])
            {
                // Calculate distances for all three forward paths
                float distanceA = Vector3.Distance(onForwardPathA[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceB = Vector3.Distance(onForwardPathB[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceC = Vector3.Distance(onForwardPathC[^1].position, nextBase.onLeftPathPoints[0].position);

                // Find the closest path
                if (distanceA <= distanceB && distanceA <= distanceC)
                {
                    path.AddRange(onForwardPathA);
                }
                else if (distanceB <= distanceA && distanceB <= distanceC)
                {
                    path.AddRange(onForwardPathB);
                }
                else
                {
                    path.AddRange(onForwardPathC);
                }
            }
            else if (startPoint == onBackwardPathA[0] || startPoint == onBackwardPathB[0] || startPoint == onBackwardPathC[0])
            {
                // Calculate distances for all three forward paths
                float distanceA = Vector3.Distance(onBackwardPathA[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceB = Vector3.Distance(onBackwardPathB[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceC = Vector3.Distance(onBackwardPathC[^1].position, nextBase.onLeftPathPoints[0].position);

                // Find the closest path
                if (distanceA <= distanceB && distanceA <= distanceC)
                {
                    path.AddRange(onBackwardPathA);
                }
                else if (distanceB <= distanceA && distanceB <= distanceC)
                {
                    path.AddRange(onBackwardPathB);
                }
                else
                {
                    path.AddRange(onBackwardPathC);
                }
            }
            else if (startPoint == onLeftPathA[0] || startPoint == onLeftPathB[0] || startPoint == onLeftPathC[0])
            {
                // Calculate distances for all three forward paths
                float distanceA = Vector3.Distance(onLeftPathA[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceB = Vector3.Distance(onLeftPathB[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceC = Vector3.Distance(onLeftPathC[^1].position, nextBase.onLeftPathPoints[0].position);

                // Find the closest path
                if (distanceA <= distanceB && distanceA <= distanceC)
                {
                    path.AddRange(onLeftPathA);
                }
                else if (distanceB <= distanceA && distanceB <= distanceC)
                {
                    path.AddRange(onLeftPathB);
                }
                else
                {
                    path.AddRange(onLeftPathC);
                }
            }
            else if (startPoint == onRightPathA[0] || startPoint == onRightPathB[0] || startPoint == onRightPathC[0])
            {
                // Calculate distances for all three forward paths
                float distanceA = Vector3.Distance(onRightPathA[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceB = Vector3.Distance(onRightPathB[^1].position, nextBase.onLeftPathPoints[0].position);
                float distanceC = Vector3.Distance(onRightPathC[^1].position, nextBase.onLeftPathPoints[0].position);

                // Find the closest path
                if (distanceA <= distanceB && distanceA <= distanceC)
                {
                    path.AddRange(onRightPathA);
                }
                else if (distanceB <= distanceA && distanceB <= distanceC)
                {
                    path.AddRange(onRightPathB);
                }
                else
                {
                    path.AddRange(onRightPathC);
                }
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
    }
}