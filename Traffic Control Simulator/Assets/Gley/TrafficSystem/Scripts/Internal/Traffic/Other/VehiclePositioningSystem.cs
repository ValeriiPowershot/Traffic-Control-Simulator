using UnityEngine;
using UnityEngine.Jobs;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Computes relative positions between cars
    /// </summary>
    public class VehiclePositioningSystem : MonoBehaviour
    {
        private TransformAccessArray allVehicles;
        private WaypointManager waypointManager;


        /// <summary>
        /// Setup method
        /// </summary>
        /// <param name="nrOfCars"></param>
        /// <param name="waypointManager"></param>
        /// <returns></returns>
        public VehiclePositioningSystem Initialize(int nrOfCars, WaypointManager waypointManager)
        {
            allVehicles = new TransformAccessArray(nrOfCars);
            this.waypointManager = waypointManager;
            return this;
        }


        /// <summary>
        /// Checks which vehicle is in front
        /// </summary>
        /// <param name="index1">index of the first vehicle to test</param>
        /// <param name="index2">index of the second vehicle to test</param>
        /// <returns>returns true if index1 is in front of index2</returns>
        public int IsInFront(int index1, int index2)
        {
            if (waypointManager.IsSameTarget(index1, index2))
            {
                //check closest distance
                if (Vector3.SqrMagnitude(allVehicles[index1].position - waypointManager.GetTargetWaypointPosition(index1)) < Vector3.SqrMagnitude(allVehicles[index2].position - waypointManager.GetTargetWaypointPosition(index2)))
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                int result = waypointManager.IsInFront(index1, index2);
                if (result == 0)
                {
                    result = CheckAngles(index1, index2);
                }
                return result;
            }
        }


        /// <summary>
        /// Used to check which vehicle is in front
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        private int CheckAngles(int index1, int index2)
        {
            //compute angles between forward vectors and relative bot position
            float angle1 = Vector3.Angle(GetForwardVector(index1), GetPosition(index2) - GetPosition(index1));
            float angle2 = Vector3.Angle(GetForwardVector(index2), GetPosition(index1) - GetPosition(index2));

            //the small angle is in front
            if (angle1 > angle2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }


        /// <summary>
        /// Check if 2 vehicles are oriented in the same direction
        /// </summary>
        /// <param name="heading1"></param>
        /// <param name="heading2"></param>
        /// <returns>true if have the same orientation</returns>
        public bool IsSameOrientation(Vector3 heading1, Vector3 heading2)
        {
            float dotResult = Vector3.Dot(heading1.normalized, heading2.normalized);
            if (dotResult > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Check if 2 vehicles are going in the same direction
        /// </summary>
        /// <param name="myHeading"></param>
        /// <param name="othervelocity"></param>
        /// <returns>true if vehicles go in the same direction</returns>
        public bool IsSameHeading(Vector3 myHeading, Vector3 othervelocity)
        {
            float dotResult = Vector3.Dot(myHeading.normalized, othervelocity.normalized);
            if (dotResult > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Update car list
        /// </summary>
        /// <param name="vehicle"></param>
        public void AddCar(Transform vehicle)
        {
            allVehicles.Add(vehicle);
        }


        /// <summary>
        /// Get up vector of the car
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetUpVector(int index)
        {
            return allVehicles[index].up;
        }


        /// <summary>
        /// Get forward vector of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetForwardVector(int index)
        {
            return allVehicles[index].forward;
        }


        /// <summary>
        /// Get right vector of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetRightVector(int index)
        {
            return allVehicles[index].right;
        }


        /// <summary>
        /// Get vehicle position in world space
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetPosition(int index)
        {
            return allVehicles[index].position;
        }


        /// <summary>
        /// Check if the velocity and orientation is the same
        /// </summary>
        /// <param name="velicity"></param>
        /// <param name="heading"></param>
        /// <returns></returns>
        public bool IsGoingForward(Vector3 velicity, Vector3 heading)
        {
            if (Vector3.Dot(velicity, heading) > -0.1f)
            {
                return true;
            }
            return false;
        }


        private void OnDestroy()
        {
            allVehicles.Dispose();
        }
    }
}