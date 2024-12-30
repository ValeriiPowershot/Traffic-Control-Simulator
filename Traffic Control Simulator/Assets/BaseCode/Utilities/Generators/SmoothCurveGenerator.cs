using UnityEngine;

namespace BaseCode.Utilities.Generators
{
    // this script generate a curve and its point between a nd b points with c angle.
    public class SmoothCurveGenerator : MonoBehaviour
    {
        public Transform positionA;
        public Transform positionMid;
        public Transform positionB;

        [SerializeField] private Transform[] _pathPoints;
        [SerializeField] private int _segments = 20;
        [SerializeField] private Color _gizmoColor = Color.red;

        private void Start()
        {
            GenerateCurve();
        }

        private void GenerateCurve()
        {
            if (positionA == null || positionMid == null || positionB == null)
            {
                Debug.LogError("Assign all positions: A, Mid, and B.");
                return;
            }

            _pathPoints = new Transform[_segments + 1];

            for (int i = 0; i <= _segments; i++)
            {
                float t = i / (float)_segments;

                // Quadratic Bezier Curve Formula
                Vector3 point = Mathf.Pow(1 - t, 2) * positionA.position +
                                2 * (1 - t) * t * positionMid.position +
                                Mathf.Pow(t, 2) * positionB.position;

                GameObject waypoint = new GameObject("Waypoint_" + i)
                {
                    transform =
                    {
                        position = point,
                        parent = transform
                    }
                };

                _pathPoints[i] = waypoint.transform;
            }
        }

        public void OnDrawGizmos()
        {
            if(positionA == null || positionMid == null || positionB == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(positionA.position, 1);
            Gizmos.DrawSphere(positionB.position, 1);
            Gizmos.DrawSphere(positionMid.position, 1);

            if (_pathPoints == null || _pathPoints.Length == 0) return;

            Gizmos.color = _gizmoColor;

            foreach (Transform point in _pathPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.2f);
            }

            for (int i = 0; i < _pathPoints.Length - 1; i++)
            {
                if (_pathPoints[i] != null && _pathPoints[i + 1] != null)
                    Gizmos.DrawLine(_pathPoints[i].position, _pathPoints[i + 1].position);
            }

        }
    }
}