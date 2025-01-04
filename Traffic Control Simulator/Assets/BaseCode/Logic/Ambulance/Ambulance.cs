using System.Collections.Generic;
using UnityEngine;
using Script.Vehicles;

namespace BaseCode.Logic.Ambulance
{
    public class Ambulance : Vehicle
    {
        [SerializeField] private GameObject pointer;
        [SerializeField] private float signsDist;

        private List<GameObject> arrows = new List<GameObject>();
        private List<GameObject> hiddenArrows = new();

        private float arriveTime;
        private const float ARRIVE_DELAY = 3f;
        private bool canArrive;

        public override void AssignNewPathContainer()
        {
            canArrive = true;
            arriveTime = Time.time + ARRIVE_DELAY;

            base.AssignNewPathContainer();

            float distance = 0;
            for(int i = 1; i < WaypointContainer.roadPoints.Count; i++)
            {
                float pointsDist = Vector3.Distance(WaypointContainer.roadPoints[i].point.position, WaypointContainer.roadPoints[i - 1].point.position);

                if (distance + pointsDist >= signsDist)
                {
                    float extraDist = distance + pointsDist - signsDist;
                    pointsDist -= extraDist;
                    pointsDist = pointsDist < 0 ? 0 : pointsDist;

                    Vector3 pos = WaypointContainer.roadPoints[i].point.position - WaypointContainer.roadPoints[i - 1].point.position;
                    pos.Normalize();
                    Quaternion rot = Quaternion.LookRotation(pos);
                    Vector3 pointerPos = WaypointContainer.roadPoints[i - 1].point.position + pos * pointsDist;
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
            if(canArrive && Time.time > arriveTime)
                canArrive = false;
            else if(!canArrive)
                base.Update();
        }

        public override void DestinationReached()
        {
            base.DestinationReached();

            foreach(var arr in arrows)
            {
                arr.SetActive(false);
                hiddenArrows.Add(arr);
            }
        }

        private void AddArrow(Vector3 pointerPos, Quaternion rot)
        {
            GameObject newArrow;
            if(hiddenArrows.Count > 0)
            {
                newArrow = hiddenArrows[0];
                newArrow.SetActive(true);
                hiddenArrows.RemoveAt(0);
            }
            else
            {
                newArrow = Instantiate(pointer);
            }

            newArrow.transform.SetPositionAndRotation(pointerPos, rot);
            arrows.Add(newArrow);
        }
    }
}
