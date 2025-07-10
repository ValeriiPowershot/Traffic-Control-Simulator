using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class SplitBezierIntoPoints
    {
        private const string helpingPointsHolderName = "HelpingPointsHolder";


        public static List<Transform> CreatePoints(RoadBase road)
        {
            GameObject helpingPointsHolder = new GameObject(helpingPointsHolderName);
            List<Transform> helpingPoints = new List<Transform>();
            helpingPointsHolder.transform.SetParent(road.transform, false);
            for (int i = 0; i < road.path.NumSegments; i++)
            {
                AddSegmentPoints(road.path, i, road.waypointDistance, helpingPointsHolder, helpingPoints, road.name, road.positionOffset);
            }
            RotateHelpingPoints(helpingPoints);
            return helpingPoints;
        }


        private static void AddSegmentPoints(Path path, int segmentIndex, float waypointDistance, GameObject helpingPointsHolder, List<Transform> helpingPoints, string roadName, Vector3 offset)
        {
            Vector3[] p = path.GetPointsInSegment(segmentIndex, Vector3.zero);
            float controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector3.Distance(p[0], p[3]) + controlNetLength / 2f;
            float nrOfWaypoints = estimatedCurveLength / waypointDistance;
            float step = 1 / nrOfWaypoints;
            float t = 0;

            while (t < 1)
            {
                AddHelpingPoint(helpingPointsHolder.transform, helpingPoints, BezierCurve.CalculateCubicBezierPoint(t, p[0], p[1], p[2], p[3]), roadName, offset);
                t += step;
            }
            AddHelpingPoint(helpingPointsHolder.transform, helpingPoints, BezierCurve.CalculateCubicBezierPoint(1, p[0], p[1], p[2], p[3]), roadName, offset);
        }


        private static void AddHelpingPoint(Transform holder, List<Transform> helpingPoints, Vector3 position, string roadName, Vector3 offset)
        {
            if (helpingPoints.Count > 0)
            {
                if (Vector3.Distance(helpingPoints[helpingPoints.Count - 1].position, position) < 0.01f)
                {
                    return;
                }
            }
            GameObject go = new GameObject();
            go.transform.position = position + offset;
            go.name = roadName + "_" + Constants.waypointNamePrefix + helpingPoints.Count;
            go.transform.SetParent(holder);
            helpingPoints.Add(go.transform);
        }


        private static void RotateHelpingPoints(List<Transform> helpingPoints)
        {
            Vector3 direction;
            for (int i = 1; i < helpingPoints.Count; i++)
            {
                direction = helpingPoints[i].position - helpingPoints[i - 1].position;
                helpingPoints[i - 1].rotation = Quaternion.LookRotation(direction);
            }
            helpingPoints[helpingPoints.Count - 1].rotation = helpingPoints[helpingPoints.Count - 2].rotation;
        }
    }
}