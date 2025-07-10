using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    public abstract class RoadBase : MonoBehaviour
    {      
        public Path path;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public float laneWidth = 4;
        public float waypointDistance = 4;
        public int nrOfLanes = 2;
        public int selectedSegmentIndex = -1;
        public bool draw;
        public bool isInsidePrefab;
        public bool inView;
        public Vector3 startPosition;
        public Vector3 endPosition;
        public bool skip;
        public bool justCreated;

        public abstract bool VerifyAssignments();

        public Path CreatePath(Vector3 startPosition, Vector3 endPosition)
        {
            path = new Path(startPosition, endPosition);
            return path;
        }

      
    }
}