#if GLEY_TRAFFIC_SYSTEM
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Applies rotation and position to the vehicle wheels 
    /// </summary>
    [BurstCompile]
    public struct UpdateWheelJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<float3> wheelsOrigin;
        [ReadOnly] public NativeArray<float3> downDirection;
        [ReadOnly] public NativeArray<float> wheelRotation;
        [ReadOnly] public NativeArray<float> turnAngle;
        [ReadOnly] public NativeArray<float> wheelRadius;
        [ReadOnly] public NativeArray<float> raycastDistance;
        [ReadOnly] public NativeArray<float> maxSuspension;
        [ReadOnly] public NativeArray<int> carIndex;
        [ReadOnly] public NativeArray<bool> canSteer;
        [ReadOnly] public int nrOfCars;

        public void Execute(int index, TransformAccess transform)
        {
            //apply suspension
            if (raycastDistance[index] != 0)
            {
                //wheel is on ground
                transform.position = wheelsOrigin[index] + downDirection[carIndex[index]] * (raycastDistance[index] - wheelRadius[index]);
            }
            else
            {
                //wheel is in the air
                transform.position = wheelsOrigin[index] + (downDirection[carIndex[index]] * maxSuspension[carIndex[index]]);
            }

            //apply rotation
            if (canSteer[index])
            {
                transform.localRotation = quaternion.EulerXYZ(math.radians(wheelRotation[carIndex[index]]), math.radians(turnAngle[carIndex[index]]), 0);
            }
            else
            {
                transform.localRotation = quaternion.EulerXYZ(math.radians(wheelRotation[carIndex[index]]), 0, 0);
            }
        }
    }
}
#endif
