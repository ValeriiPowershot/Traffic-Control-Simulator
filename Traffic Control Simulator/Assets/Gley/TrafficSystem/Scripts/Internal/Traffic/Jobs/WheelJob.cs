#if GLEY_TRAFFIC_SYSTEM
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Compotes the suspension force for each wheel
    /// </summary>
    [BurstCompile]
    public struct WheelJob : IJobParallelFor
    {
        public NativeArray<float3> wheelSuspensionForce;
        public NativeArray<float3> wheelSideForce;

        [ReadOnly] public NativeArray<float3> wheelNormalDirection;
        [ReadOnly] public NativeArray<float3> wheelRightDirection;
        [ReadOnly] public NativeArray<float3> wheelVelocity;
        [ReadOnly] public NativeArray<float> springForces;
        [ReadOnly] public NativeArray<float> wheelRaycastDistance;
        [ReadOnly] public NativeArray<float> wheelRadius;
        [ReadOnly] public NativeArray<float> wheelMaxSuspension;
        [ReadOnly] public NativeArray<float> springStiffness;
        [ReadOnly] public NativeArray<int> startWheelIndex;
        [ReadOnly] public NativeArray<int> nrOfCarWheels;
        [ReadOnly] public NativeArray<int> wheelAssociatedCar;
        [ReadOnly] public NativeArray<int> vehicleNrOfWheels;
        public void Execute(int i)
        {
            float compression;
            if (wheelMaxSuspension[i] != 0)
            {
                if (wheelRaycastDistance[i] == 0)
                {
                    compression = 0;
                }
                else
                {
                    compression = 1f - (wheelRaycastDistance[i] - wheelRadius[i]) / wheelMaxSuspension[i];
                }
                wheelSuspensionForce[i] = ComputeSuspensionForce(springForces[i], compression, wheelNormalDirection[i], i);
            }
            else
            {
                compression = 1;
                wheelSuspensionForce[i] = ComputeSuspensionForce(springForces[i], 1, wheelNormalDirection[i], i);
            }
            wheelSideForce[i] = -wheelRightDirection[i] * Vector3.Dot(wheelVelocity[i], wheelRightDirection[i]) / vehicleNrOfWheels[wheelAssociatedCar[i]];
        }


        float3 ComputeSuspensionForce(float springForce, float compression, float3 normalPoint, int index)
        {
            float damping = wheelVelocity[index].y * springForce / 2;

            float displacement = 0.5f - compression;
            if (displacement > -0.2f)
            {
                displacement = 0;
            }      

            float force = ((springForce * (compression / 0.5f) - springForce * springStiffness[index] * displacement) * normalPoint).y - damping;
            return new float3(0, force, 0);
        }
    }
}
#endif
