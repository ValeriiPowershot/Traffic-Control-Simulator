using UnityEngine;

namespace BaseCode.Logic.CarMovement
{
    public class PathGenerator : MonoBehaviour
    {
        [SerializeField] private Transform[] _pathPoints;
        [SerializeField] private float _radius = 5f;
        [SerializeField] private int _segments = 20;
        [SerializeField] private Color _gizmoColor = Color.red; 

        private void Start()
        {
            _pathPoints = new Transform[_segments + 1];

            for (int i = 0; i <= _segments; i++)
            {
                float angle = Mathf.Deg2Rad * (i * 90f / _segments);

                Vector3 point = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * _radius;

                GameObject waypoint = new GameObject("Waypoint_" + i);
                waypoint.transform.position = transform.position + point;
                waypoint.transform.parent = transform;

                _pathPoints[i] = waypoint.transform;
            }
        }
        
        private void OnDrawGizmos()
        {
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
