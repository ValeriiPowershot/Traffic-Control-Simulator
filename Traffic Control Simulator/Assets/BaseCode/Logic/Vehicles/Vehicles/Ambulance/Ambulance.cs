using System.Collections;
using System.Collections.Generic;
using BaseCode.Logic.Vehicles.Controllers.Collision;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.Vehicles.Vehicles.Ambulance
{
    public class Ambulance : BasicCar
    {
        [SerializeField] private GameObject arrowPrefab;
        private readonly List<GameObject> _arrows = new();

        private const float MinDistanceToSpawnArrow = 4; // this is tested value, looks nice :) (not like u)
        private GameObject _newArrow;
        protected override void AssignCollisionController()
        {
            VehicleController.VehicleCollisionController = new AmbulanceVehicleCollisionController();
            VehicleController.VehicleCollisionController.Starter(this);
        }

        public override void StartToMove()
        {
            base.StartToMove();
            StartCoroutine(OpenSetActiveOfArrows());
        }
        public override void AssignNewPathContainer()
        {
            base.AssignNewPathContainer();
            ProcessPathSegments();
        }

        private void ProcessPathSegments()
        {
            var roadPoints = PathContainerService.GetPathContainer().roadPoints;
            float accumulatedDistance = 0f;

            _newArrow = new GameObject("Arrows");

            for (int i = 1; i < roadPoints.Count; i++)  
            {
                Vector3 currentSegment = GetIndex(i - 1).position;
                Vector3 nextSegment = GetIndex(i).position;
                float segmentDistance = Vector3.Distance(currentSegment, nextSegment);

                accumulatedDistance += segmentDistance;
                if (IsThereEnoughIntervalBetweenArrows(accumulatedDistance, segmentDistance))
                {
                    AddArrow(nextSegment, currentSegment);
                    accumulatedDistance = 0f; 
                }
            }

        }
        
        private IEnumerator OpenSetActiveOfArrows()
        {
            Vector3 targetScale = new Vector3(0.3f, 0.3f, 0.3f); // original was 0.3f sorry for this imp
            foreach (var arrow in _arrows)
            {
                arrow.SetActive(true);
                arrow.transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void AddArrow(Vector3 nextSegment, Vector3 currentSegment)
        {
            Vector3 rotVector = (nextSegment - currentSegment).normalized;
            Quaternion rot = Quaternion.LookRotation(rotVector);

            Vector3 arrowPrefabPos = currentSegment;
            arrowPrefabPos.y = transform.position.y / 2;
            
            SpawnArrow(arrowPrefabPos, rot);
        }
        private bool IsThereEnoughIntervalBetweenArrows(float accumulatedDistance, float segmentDistance)
        {
            return accumulatedDistance >= MinDistanceToSpawnArrow;
        }

        public override void DestinationReached()
        {
            base.DestinationReached();

            foreach(var arr in _arrows)
            {
                Destroy(arr);
            }
            Destroy(_newArrow);
            _arrows.Clear();
            Debug.Log("ASd");
        }
        private void SpawnArrow(Vector3 arrowPrefabPos, Quaternion rot)
        {
            GameObject newArrow = Instantiate(arrowPrefab,_newArrow.transform);
            newArrow.transform.SetPositionAndRotation(arrowPrefabPos, rot);
            _arrows.Add(newArrow);
            newArrow.gameObject.SetActive(false);
            newArrow.transform.localScale = Vector3.zero;
        }
  

        private Transform GetIndex(int index)
        {
            return PathContainerService.GetIndexWaypoint(index).point;
        }
    }
}
