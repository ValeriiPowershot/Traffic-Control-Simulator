using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Used to check if a vehicle can be instantiated in a given position
    /// </summary>
    public class PositionValidator : MonoBehaviour
    {
        private LayerMask trafficLayer;
        private LayerMask playerLayer;
        private LayerMask buildingsLayers;
        private Transform[] activeCameras;
        private float minDistanceToAdd;
        private bool debugDensity;

        private Vector3 debugPosition;
        private Matrix4x4 matrix;
        private float vehicleLength;
        private float vehicleWidth;
        private float vehicleHeight;


        /// <summary>
        /// Setup dependencies
        /// </summary>
        /// <param name="activeCameras"></param>
        /// <param name="trafficLayer"></param>
        /// <param name="buildingsLayers"></param>
        /// <param name="minDistanceToAdd"></param>
        /// <param name="debugDensity"></param>
        /// <returns></returns>
        public PositionValidator Initialize(Transform[] activeCameras, LayerMask trafficLayer, LayerMask playerLayer, LayerMask buildingsLayers, float minDistanceToAdd, bool debugDensity)
        {
            UpdateCamera(activeCameras);
            this.trafficLayer = trafficLayer;
            this.playerLayer = playerLayer;
            this.minDistanceToAdd = minDistanceToAdd * minDistanceToAdd;
            this.buildingsLayers = buildingsLayers;
            this.debugDensity = debugDensity;
            return this;
        }


        /// <summary>
        /// Checks if a vehicle can be instantiated in a given position
        /// </summary>
        /// <param name="position">position to check</param>
        /// <param name="vehicleLength"></param>
        /// <param name="vehicleHeight"></param>
        /// <param name="ignoreLineOfSight">validate position eve if it is in view</param>
        /// <returns></returns>
        public bool IsValid(Vector3 position, float vehicleLength, float vehicleHeight, float vehicleWidth, bool ignoreLineOfSight, float frontWheelOffset, Quaternion rotation)
        {
            position -= rotation * new Vector3(0, 0, frontWheelOffset);
            for (int i = 0; i < activeCameras.Length; i++)
            {
                if (!ignoreLineOfSight)
                {
                    //if position if far enough from the player
                    if (Vector3.SqrMagnitude(activeCameras[i].position - position) < minDistanceToAdd)
                    {
                        if (!Physics.Linecast(position, activeCameras[i].position, buildingsLayers))
                        {
#if UNITY_EDITOR
                            if (debugDensity)
                            {
                                Debug.DrawLine(activeCameras[i].position, position, Color.red, 0.1f);
                            }
#endif
                            return false;
                        }
                        else
                        {
#if UNITY_EDITOR
                            if (debugDensity)
                            {
                                Debug.DrawLine(activeCameras[i].position, position, Color.green, 0.1f);
                            }
#endif
                        }
                    }
                }
            }

            //check if the final position is free 
            return IsPositionFree(position, vehicleLength, vehicleHeight, vehicleWidth, rotation);
        }


        /// <summary>
        /// Check if a given position if free
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public bool IsPositionFree(Vector3 position, float length, float height, float width, Quaternion rotation)
        {
            Collider[] colliders = Physics.OverlapBox(position, new Vector3(width / 2, height / 2, length / 2), rotation, trafficLayer | playerLayer);
            if (Physics.OverlapBox(position, new Vector3(width / 2, height / 2, length / 2), rotation, trafficLayer | playerLayer).Length > 0)
            {
#if UNITY_EDITOR
                if (debugDensity)
                {
                    matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
                    debugPosition = position;
                    vehicleLength = length;
                    vehicleHeight = height;
                    vehicleWidth = width;
                }
#endif
                return false;
            }
            debugPosition = Vector3.zero;
            return true;
        }

        internal bool CheckTrailerPosition(Vector3 position, Quaternion vehicleRotation, Quaternion trailerRotation, VehicleComponent vehicle)
        {
            Vector3 translatedPosition = position - vehicleRotation * Vector3.forward * (vehicle.frontTrigger.transform.localPosition.z + vehicle.carHolder.transform.localPosition.z);
            translatedPosition = translatedPosition - trailerRotation * Vector3.forward * vehicle.trailer.length / 2;
            return IsPositionFree(translatedPosition, vehicle.trailer.length, vehicle.trailer.height, vehicle.trailer.width, trailerRotation);
        }


        /// <summary>
        /// Update player camera transform
        /// </summary>
        /// <param name="activeCameras"></param>
        public void UpdateCamera(Transform[] activeCameras)
        {
            this.activeCameras = activeCameras;
        }


        //debug
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugPosition != Vector3.zero)
            {
                UnityEditor.Handles.matrix = matrix;
                UnityEditor.Handles.DrawWireCube(Vector3.zero, new Vector3(vehicleWidth, vehicleHeight, vehicleLength));
            }
        }
#endif
    }
}
