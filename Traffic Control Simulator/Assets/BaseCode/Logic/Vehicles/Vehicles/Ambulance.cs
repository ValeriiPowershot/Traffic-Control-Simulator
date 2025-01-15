using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Vehicles.Vehicles
{
    public class Ambulance : BasicCar
    {
        [SerializeField] private GameObject _pointer;
        [SerializeField] private float _signsDist;
        [Range(0, 16)] [SerializeField] private float _spawnDelay;

        private List<GameObject> _arrows = new();
        private List<GameObject> _hiddenArrows = new();

        private float _arriveTime;
        private bool _canArrive;

        public override void AssignNewPathContainer()
        {
            _canArrive = true;
            _arriveTime = Time.time - _spawnDelay;

            base.AssignNewPathContainer();

            float distance = 0;
            
            for(int i = 1; i < PathContainerService.GetPathContainer().roadPoints.Count; i++)
            {
                float pointsDist = Vector3.Distance(PathContainerService.GetIndexWaypoint(i).point.position, 
                    PathContainerService.GetIndexWaypoint(i - 1).point.position);

                if (distance + pointsDist >= _signsDist)
                {
                    float extraDist = distance + pointsDist - _signsDist;
                    pointsDist -= extraDist;
                    pointsDist = pointsDist < 0 ? 0 : pointsDist;

                    Vector3 pos = PathContainerService.GetIndexWaypoint(i).point.position
                                  - PathContainerService.GetIndexWaypoint(i - 1).point.position;
                    pos.Normalize();
                    Quaternion rot = Quaternion.LookRotation(pos);
                    Vector3 pointerPos = PathContainerService.GetIndexWaypoint(i - 1).point.position + pos * pointsDist;
                    pointerPos.y = transform.position.y + 0.1f;

                    AddArrow(pointerPos, rot);

                    distance = extraDist;
                }
                else
                    distance += pointsDist;
            }
        }

        public override void Update()
        {
            if(_canArrive && Time.time > _arriveTime)
                _canArrive = false;
            if(!_canArrive)
                base.Update();
        }

        public override void DestinationReached()
        {
            base.DestinationReached();

            foreach(GameObject arr in _arrows)
            {
                arr.SetActive(false);
                _hiddenArrows.Add(arr);
            }
        }

        private void AddArrow(Vector3 pointerPos, Quaternion rot)
        {
            GameObject newArrow;
            if(_hiddenArrows.Count > 0)
            {
                newArrow = _hiddenArrows[0];
                newArrow.SetActive(true);
                _hiddenArrows.RemoveAt(0);
            }
            else
            {
                newArrow = Instantiate(_pointer);
            }

            newArrow.transform.SetPositionAndRotation(pointerPos, rot);
            _arrows.Add(newArrow);
        }
    }
}
