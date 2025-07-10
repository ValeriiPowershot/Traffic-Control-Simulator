using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Keeps track of all traffic vehicles
    /// </summary>
    public class TrafficVehicles : MonoBehaviour
    {
        private List<VehicleComponent> allVehicles = new List<VehicleComponent>();
        private List<VehicleComponent> idleVehicles = new List<VehicleComponent>();
        private Transform trafficHolder;
        private float masterVolume;
        private float realtimeSinceStartup;


        /// <summary>
        /// Loads all traffic vehicles
        /// </summary>
        /// <param name="vehiclePool"></param>
        /// <param name="nrOfVehicles"></param>
        /// <param name="buildingLayers"></param>
        /// <param name="obstacleLayers"></param>
        /// <param name="playerLayers"></param>
        /// <param name="masterVolume"></param>
        /// <returns></returns>
        public TrafficVehicles Initialize(VehiclePool vehiclePool, int nrOfVehicles, LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, LayerMask roadLayers, float masterVolume, bool lightsOn, ModifyTriggerSize modifyTriggerSize)
        {
            if (trafficHolder == null)
            {
                trafficHolder = new GameObject(UrbanAssets.Internal.Constants.trafficHolderName).transform;
            }

            this.masterVolume = masterVolume;

            //transform percent into numbers;
            int carsToInstantiate = vehiclePool.trafficCars.Length;
            if (carsToInstantiate > nrOfVehicles)
            {
                carsToInstantiate = nrOfVehicles;
            }
            for (int i = 0; i < carsToInstantiate; i++)
            {
                LoadCar(vehiclePool.trafficCars[i].vehiclePrefab, buildingLayers, obstacleLayers, playerLayers, roadLayers, vehiclePool.trafficCars[i].dontInstantiate, lightsOn, modifyTriggerSize);
            }

            nrOfVehicles -= carsToInstantiate;
            float sum = 0;
            List<float> thresholds = new List<float>();
            for (int i = 0; i < vehiclePool.trafficCars.Length; i++)
            {
                sum += vehiclePool.trafficCars[i].percent;
                thresholds.Add(sum);
            }
            float perCarValue = sum / nrOfVehicles;

            //load cars
            int vehicleIndex = 0;
            for (int i = 0; i < nrOfVehicles; i++)
            {
                while ((i + 1) * perCarValue > thresholds[vehicleIndex])
                {
                    vehicleIndex++;
                    if (vehicleIndex >= vehiclePool.trafficCars.Length)
                    {
                        vehicleIndex = vehiclePool.trafficCars.Length - 1;
                        break;
                    }
                }
                LoadCar(vehiclePool.trafficCars[vehicleIndex].vehiclePrefab, buildingLayers, obstacleLayers, playerLayers, roadLayers, vehiclePool.trafficCars[vehicleIndex].dontInstantiate, lightsOn, modifyTriggerSize);
            }
            return this;
        }


        /// <summary>
        /// Get entire vehicle list
        /// </summary>
        /// <returns></returns>
        public List<VehicleComponent> GetVehicleList()
        {
            return allVehicles;
        }


        public VehicleComponent GetVehicleComponent(int index)
        {
            return allVehicles[index];
        }


        public List<VehicleComponent> GetExcludedVehicleList()
        {
            List<VehicleComponent> result = new List<VehicleComponent>();
            for (int i = 0; i < allVehicles.Count; i++)
            {
                if (allVehicles[i].excluded)
                {
                    result.Add(allVehicles[i]);
                }
            }
            return result;
        }


        /// <summary>
        /// Set reverse lights if required on a specific vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public void SetReverseLights(int index, bool active)
        {
            allVehicles[index].SetReverseLights(active);
        }


        /// <summary>
        /// Set brake lights if required on a specific vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="active"></param>
        public void SetBrakeLights(int index, bool active)
        {
            allVehicles[index].SetBrakeLights(active);
        }


        /// <summary>
        /// Get an available vehicle to be instantiated
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public VehicleComponent GetIdleVehicle(int vehicleIndex)
        {
            VehicleComponent vehicle = idleVehicles[vehicleIndex];
            idleVehicles.RemoveAt(vehicleIndex);
            return vehicle;
        }


        public VehicleComponent PeakIdleVehicle(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex];
        }


        public void RemoveIdleVehicle(int vehicleIndex)
        {
            idleVehicles.RemoveAt(vehicleIndex);
        }


        public bool VehicleIsExcluded(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].excluded;
        }


        public VehicleComponent GetExcludedVehicle(int vehicleIndex)
        {
            return allVehicles[vehicleIndex];
        }


        public int GetIdleVehicleIndex()
        {
            if (idleVehicles.Count <= 0)
            {
                return -1;
            }
            return Random.Range(0, idleVehicles.Count);
        }


        /// <summary>
        /// Get a random index of an idle vehicle
        /// </summary>
        /// <returns></returns>
        public int GetIdleVehicleIndex(VehicleTypes type)
        {
            var possibleVehicles = idleVehicles.Where(cond => cond.vehicleType == type).ToArray();

            if (possibleVehicles.Length > 0)
            {
                return idleVehicles.IndexOf(possibleVehicles[Random.Range(0, possibleVehicles.Length)]);
            }

            return -1;
        }


        /// <summary>
        /// Get the vehicle type of a given vehicle index
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public VehicleTypes GetIdleVehicleType(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].vehicleType;
        }


        public VehicleTypes GetVehicleType(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].vehicleType;
        }


        public float GetFrontWheelOffset(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].frontTrigger.localPosition.z;
        }


        /// <summary>
        /// Remove the given vehicle from scene
        /// </summary>
        /// <param name="index"></param>
        public void RemoveVehicle(int index)
        {
            if (!allVehicles[index].excluded)
            {
                idleVehicles.Add(allVehicles[index]);
            }

            allVehicles[index].DeactivateVehicle();

            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].ColliderRemoved(GetAllColliders(index));
            }
        }


        internal void ExcludeVehicleFromSystem(int vehicleIndex)
        {
            allVehicles[vehicleIndex].excluded = true;
            idleVehicles.Remove(allVehicles[vehicleIndex]);
        }


        internal void AddExcludecVehicleToSystem(int vehicleIndex)
        {
            allVehicles[vehicleIndex].excluded = false;
            if (!idleVehicles.Contains(allVehicles[vehicleIndex]) && allVehicles[vehicleIndex].gameObject.activeSelf == false)
            {
                idleVehicles.Add(allVehicles[vehicleIndex]);
            }
        }


        /// <summary>
        /// Check if the given vehicle can be removed from scene
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CanBeRemoved(int index)
        {
            return allVehicles[index].CanBeRemoved();
        }


        /// <summary>
        /// Get the current velocity of the given vehicle index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetVelocity(int index)
        {
            return allVehicles[index].GetVelocity();
        }


        /// <summary>
        /// Get the speed of the given vehicle index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetCurrentSpeed(int index)
        {
            return allVehicles[index].GetCurrentSpeed();
        }


        //if a vehicle has a traffic participant in trigger, it will return the minimum speed of all participants
        public float GetFollowSpeed(int index)
        {
            return allVehicles[index].GetFollowSpeed();
        }


        /// <summary>
        /// Get the length of the given vehicle index
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public float GetHalfVehicleLength(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].length / 2;
        }


        public float GetVehicleLength(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].length;
        }


        /// <summary>
        /// Get the height of the given vehicle index
        /// </summary>
        /// <param name="vehicleIndex"></param>
        /// <returns></returns>
        public float GetIdleVehicleHeight(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].coliderHeight;
        }


        public float GetIdleVehicleWidth(int vehicleIndex)
        {
            return idleVehicles[vehicleIndex].colliderWidth / 2;
        }


        public float GetVehicleWidth(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].colliderWidth;
        }


        /// <summary>
        /// Activate a vehicle on scene
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="position"></param>
        /// <param name="vehicleRotation"></param>
        public void ActivateVehicle(VehicleComponent vehicle, Vector3 position, Quaternion vehicleRotation, Quaternion trailerRotation)
        {
            vehicle.ActivateVehicle(position, vehicleRotation, masterVolume, trailerRotation);
            idleVehicles.Remove(vehicle);
        }


        /// <summary>
        /// Get the vehicle collider
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Collider GetCollider(int index)
        {
            return allVehicles[index].GetCollider();
        }


        /// <summary>
        /// Get the vehicle moving direction
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetHeading(int index)
        {
            return allVehicles[index].GetHeading();
        }


        /// <summary>
        /// Get the vehicles forward direction
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetForwardVector(int index)
        {
            return allVehicles[index].GetForwardVector();
        }


        /// <summary>
        /// Set a new action for a vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="currentAction"></param>
        public void SetCurrentAction(int index, TrafficSystem.DriveActions currentAction)
        {
            allVehicles[index].SetCurrentAction(currentAction);
        }


        /// <summary>
        /// The give vehicle has stopped reversing
        /// </summary>
        /// <param name="index"></param>
        public void CurrentVehicleActionDone(int index)
        {
            allVehicles[index].CurrentVehicleActionDone();
        }


        /// <summary>
        /// Get the current action for the given vehicle index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TrafficSystem.DriveActions GetCurrentAction(int index)
        {
            return allVehicles[index].GetCurrentAction();
        }


        /// <summary>
        /// Get the vehicles max speed
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetMaxSpeed(int index)
        {
            return allVehicles[index].GetMaxSpeed();
        }


        /// <summary>
        /// ???
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetPossibleMaxSpeed(int index)
        {
            return allVehicles[index].maxPossibleSpeed;
        }


        /// <summary>
        /// Set the corresponding blinker for the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="blinkType"></param>
        public void SetBlinkLights(int index, BlinkType blinkType)
        {
            allVehicles[index].SetBlinker(blinkType);
        }


        /// <summary>
        /// Load car in scene
        /// </summary>
        /// <param name="carPrefab"></param>
        /// <param name="buildingLayers"></param>
        /// <param name="obstacleLayers"></param>
        /// <param name="playerLayers"></param>
        private void LoadCar(GameObject carPrefab, LayerMask buildingLayers, LayerMask obstacleLayers, LayerMask playerLayers, LayerMask roadLayers, bool excluded, bool lightsOn, ModifyTriggerSize modifyTriggerSize)
        {
            VehicleComponent car = Instantiate(carPrefab, Vector3.zero, Quaternion.identity, trafficHolder).GetComponent<VehicleComponent>().Initialize(buildingLayers, obstacleLayers, playerLayers, roadLayers, lightsOn, modifyTriggerSize);
            car.SetIndex(allVehicles.Count);
            car.name += allVehicles.Count;
            car.excluded = excluded;
            allVehicles.Add(car);
            RemoveVehicle(car.GetIndex());
        }


        /// <summary>
        /// Get the spring force of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetSpringForce(int index)
        {
            return allVehicles[index].GetSpringForce();
        }


        /// <summary>
        /// Get the power step (acceleration) of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetPowerStep(int index)
        {
            return allVehicles[index].GetPowerStep();
        }


        /// <summary>
        /// Get the brake power step of the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetBrakeStep(int index)
        {
            return allVehicles[index].GetBrakeStep();
        }


        /// <summary>
        /// Get ground orientation vector
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetGroundDirection(int index)
        {
            return allVehicles[index].GetGroundDirection();
        }


        /// <summary>
        /// Update additional components from the vehicle if needed
        /// </summary>
        /// <param name="index"></param>
        public void UpdateVehicleScripts(int index)
        {
            if (index == 0)
            {
                realtimeSinceStartup += Time.deltaTime;
            }
            allVehicles[index].UpdateEngineSound(masterVolume);
            allVehicles[index].UpdateLights(realtimeSinceStartup);
            allVehicles[index].UpdateColliderSize();
        }


        /// <summary>
        /// Update main lights of the vehicle
        /// </summary>
        /// <param name="on"></param>
        public void UpdateVehicleLights(bool on)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                allVehicles[i].SetMainLights(on);
            }
        }


        /// <summary>
        /// Update engine volume of the vehicle
        /// </summary>
        /// <param name="volume"></param>
        public void UpdateMasterVolume(float volume)
        {
            this.masterVolume = volume;
        }


        internal void TriggerColliderRemovedEvent(Collider[] colliders)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                if (allVehicles[i])
                {
                    allVehicles[i].ColliderRemoved(colliders);
                }
            }
        }


        internal float GetTriggerSize(int index)
        {
            return allVehicles[index].GetTriggerSize();
        }


        internal void ModifyTriggerSize(int vehicleIndex, ModifyTriggerSize modifyTriggerSizeDelegate)
        {
            if (vehicleIndex < 0)
            {
                for (int i = 0; i < allVehicles.Count; i++)
                {
                    allVehicles[i].SetTriggerSizeModifierDelegate(modifyTriggerSizeDelegate);
                }
            }
            else
            {
                allVehicles[vehicleIndex].SetTriggerSizeModifierDelegate(modifyTriggerSizeDelegate);
            }
        }
#if GLEY_TRAFFIC_SYSTEM
        internal Unity.Mathematics.float3 GetClosestObstacle(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].GetClosestObstacle();
        }
#endif


        internal Collider[] GetAllColliders(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].GetAllColliders();
        }


        internal void SetMaxSpeed(int vehicleIndex, float speed)
        {
            allVehicles[vehicleIndex].SetMaxSpeed(speed);
        }


        internal void ResetMaxSpeed(int vehicleIndex)
        {
            allVehicles[vehicleIndex].ResetMaxSpeed();
        }


        internal bool HasTrailer(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].trailer != null;
        }


        internal int GetTrailerWheels(int vehicleIndex)
        {
            return allVehicles[vehicleIndex].trailer.GetNrOfWheels();
        }


        internal int GetVehicleIndex(GameObject vehicle)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                if (allVehicles[i].gameObject == vehicle)
                {
                    return i;
                }
            }
            return -1;
        }


        internal int GetExcludedVehicleIndex(GameObject vehicle)
        {
            for (int i = 0; i < allVehicles.Count; i++)
            {
                if (allVehicles[i].excluded)
                {
                    if (allVehicles[i].gameObject == vehicle)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
