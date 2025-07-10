using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    [System.Serializable]
    public class Waypoint : WaypointBase
    {
        public List<int> giveWayList;
        public List<int> allowedAgents;
        public List<int> otherLanes;
        public int maxSpeed;
        public float laneWidth;
        public bool giveWay;
        public bool complexGiveWay;
        public bool zipperGiveWay;
        public bool triggerEvent;
        public string eventData;
        public bool enter;
        public bool exit;
        public bool stop;
        private IIntersection associatedIntersection;
        /// <summary>
        /// Constructor used to convert from editor waypoint to runtime waypoint 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="listIndex"></param>
        /// <param name="position"></param>
        /// <param name="allowedCars"></param>
        /// <param name="neighbors"></param>
        /// <param name="prev"></param>
        /// <param name="otherLanes"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="stop"></param>
        /// <param name="giveWay"></param>
        /// <param name="enter"></param>
        /// <param name="exit"></param>
        public Waypoint(string name,
            int listIndex,
            Vector3 position,
            List<VehicleTypes> allowedCars,
            List<int> neighbors,
            List<int> prev,
            List<int> otherLanes,
            int maxSpeed, 
            int priority,
            bool giveWay,
            bool complexGiveWay,
            bool zipperGiveWay,
            bool triggerEvent,
            float laneWidth,
            string eventData,
            List<int> giveWayList) : base(name, listIndex, position, neighbors, prev)
        {
            this.maxSpeed = maxSpeed;
            this.giveWay = giveWay;
            this.complexGiveWay = complexGiveWay;
            this.zipperGiveWay = zipperGiveWay;
            this.giveWayList = giveWayList;
            this.laneWidth = laneWidth;
            this.otherLanes = otherLanes;
            enter = false;
            exit = false;
            stop = false;
            this.triggerEvent = triggerEvent;
            this.eventData = eventData;
            this.allowedAgents = allowedCars.Cast<int>().ToList();
        }


        public Waypoint()
        {

        }


        /// <summary>
        /// Initializes current waypoint properties
        /// Used by intersections
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="giveWay"></param>
        /// <param name="stop"></param>
        /// <param name="enter"></param>
        /// <param name="exit"></param>
        public void SetIntersection(IIntersection intersection, bool giveWay, bool stop, bool enter, bool exit)
        {
            associatedIntersection = intersection;
            this.stop = stop;
            this.giveWay = giveWay;
            this.exit = exit;
            this.enter = enter;
        }


        /// <summary>
        /// Check if the waypoint is free
        /// </summary>
        /// <returns>true if intersection allows passing through this waypoint</returns>
        public bool CanChange()
        {
            return associatedIntersection.IsPathFree(listIndex);
        }


        /// <summary>
        /// Check if the waypoint belongs to an intersection
        /// </summary>
        /// <returns></returns>
        public bool IsInIntersection()
        {
            return associatedIntersection != null;
        }


        /// <summary>
        /// Waypoint is no longer a target for the vehicle
        /// </summary>
        internal override void Passed(int vehicleIndex)
        {
            if (exit)
            {
                associatedIntersection.VehicleLeft(vehicleIndex);
                return;
            }
            if (enter)
            {
                associatedIntersection.VehicleEnter(vehicleIndex);
                return;
            }
            if(triggerEvent == true)
            {
                Events.TriggerWaypointReachedEvent(vehicleIndex, listIndex, eventData);
            }
        }
    }
}