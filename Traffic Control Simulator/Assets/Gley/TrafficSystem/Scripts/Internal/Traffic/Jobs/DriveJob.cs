#if GLEY_TRAFFIC_SYSTEM
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Handles driving part of the vehicle
    /// </summary>
    [BurstCompile]
    public struct DriveJob : IJobParallelFor
    {
        public NativeArray<float3> bodyForce;
        public NativeArray<float3> trailerForce;
        public NativeArray<float> actionValue;
        public NativeArray<float> wheelRotation;
        public NativeArray<float> turnAngle;
        public NativeArray<float> vehicleRotationAngle;
        public NativeArray<int> gear;
        public NativeArray<bool> readyToRemove;
        public NativeArray<bool> needsWaypoint;
        public NativeArray<bool> isBraking;

        [ReadOnly] public NativeArray<TrafficSystem.DriveActions> specialDriveAction;
        [ReadOnly] public NativeArray<float3> targetWaypointPosition;
        [ReadOnly] public NativeArray<float3> allBotsPosition;
        [ReadOnly] public NativeArray<float3> groundDirection;
        [ReadOnly] public NativeArray<float3> forwardDirection;
        [ReadOnly] public NativeArray<float3> rightDirection;
        [ReadOnly] public NativeArray<float3> trailerRightDirection;
        [ReadOnly] public NativeArray<float3> trailerForwardDirection;
        [ReadOnly] public NativeArray<float3> triggerForwardDirection;
        [ReadOnly] public NativeArray<float3> downDirection;
        [ReadOnly] public NativeArray<float3> carVelocity;
        [ReadOnly] public NativeArray<float3> trailerVelocity;
        [ReadOnly] public NativeArray<float3> cameraPositions;
        [ReadOnly] public NativeArray<float3> closestObstacle;
        [ReadOnly] public NativeArray<float> wheelCircumferences;
        [ReadOnly] public NativeArray<float> maxSteer;
        [ReadOnly] public NativeArray<float> powerStep;
        [ReadOnly] public NativeArray<float> brakeStep;
        [ReadOnly] public NativeArray<float> drag;
        [ReadOnly] public NativeArray<float> trailerDrag;
        [ReadOnly] public NativeArray<float> maxSpeed;
        [ReadOnly] public NativeArray<float> wheelDistance;
        [ReadOnly] public NativeArray<float> steeringStep;
        [ReadOnly] public NativeArray<float> vehicleLength;
        [ReadOnly] public NativeArray<float> massDifference;
        [ReadOnly] public NativeArray<float> distanceToStop;
        [ReadOnly] public NativeArray<int> wheelSign;
        [ReadOnly] public NativeArray<int> nrOfWheels;
        [ReadOnly] public NativeArray<int> trailerNrOfWheels;
        [ReadOnly] public float3 worldUp;
        [ReadOnly] public float distanceToRemove;
        [ReadOnly] public float fixedDeltaTime;


        private float3 waypointDirection;
        private float minWaypointDistance;
        private float _targetSpeed; //required car speed in next frame
        private float currentSpeed; //speed in current frame
        private float dotProduct;
        private float waypointDistance;
        private float angle;
        private bool avoidBackward;
        private bool avoidForward;


        public void Execute(int index)
        {
            //reset variables
            avoidForward = false;
            avoidBackward = false;
            isBraking[index] = false;

            //compute current frame values
            float3 forwardVelocity = forwardDirection[index] * Vector3.Dot(carVelocity[index], forwardDirection[index]);
            _targetSpeed = math.length(forwardVelocity);
            currentSpeed = _targetSpeed * math.sign(Vector3.Dot(forwardVelocity, forwardDirection[index]));
            waypointDirection = targetWaypointPosition[index] - allBotsPosition[index];
            dotProduct = math.dot(waypointDirection, forwardDirection[index]);// used to check if vehicle passed the current waypoint
            waypointDirection.y = 0;
            waypointDistance = math.distance(targetWaypointPosition[index], allBotsPosition[index]);

            //Debug.Log(forwardDirection[index]);

            //change the distance to change waypoints based on vehicle speed
            //at 50 kmh -> min distance =1.5
            //at 100 kmh -> min distance =2.5
            //kmh to ms => 50/3.6 = 13.88
            if (currentSpeed < 13.88f)
            {
                minWaypointDistance = 1.5f;
            }
            else
            {
                minWaypointDistance = 1.5f + (currentSpeed * 3.6f - 50) / 50;
            }

            //compute acceleration based on the current vehicle actions
            Drive(index);

            //compute forces required for the target speed to be achieved
            ComputeBodyForce(index, maxSpeed[index], gear[index]);

            //check if a new waypoint is required
            ChangeWaypoint(index);

            //compute the wheel turn amount
            ComputeWheelRotationAngle(index);

            //compute steering angle
            ComputeSteerAngle(index, maxSteer[index], _targetSpeed);

            //check if vehicle is far enough for the player and it can be removed
            RemoveVehicle(index);
        }


        #region Drive
        /// <summary>
        /// Compute acceleration value based on the current vehicle`s driving actions
        /// </summary>
        /// <param name="index">index of the current vehicle </param>
        private void Drive(int index)
        {
            if (actionValue[index] != math.INFINITY)
            {
                actionValue[index] -= fixedDeltaTime;
            }

            switch (specialDriveAction[index])
            {
                case TrafficSystem.DriveActions.Reverse:
                    Reverse(index);
                    break;
                case TrafficSystem.DriveActions.AvoidReverse:
                    AvoidReverse(index);
                    break;
                case TrafficSystem.DriveActions.StopTemp:
                case TrafficSystem.DriveActions.NoWaypoint:
                case TrafficSystem.DriveActions.NoPath:
                case TrafficSystem.DriveActions.Stop:
                    StopNow(index);
                    break;
                case TrafficSystem.DriveActions.StopInDistance:
                    StopInDistance(index);
                    break;
                case TrafficSystem.DriveActions.AvoidForward:
                    AvoidForward(index);
                    break;
                case TrafficSystem.DriveActions.StopInPoint:
                case TrafficSystem.DriveActions.GiveWay:
                    StopInPoint(index);
                    break;
                case TrafficSystem.DriveActions.Follow:
                case TrafficSystem.DriveActions.Overtake:
                    Follow(index, maxSpeed[index]);
                    break;
                default:
                    Forward(index, maxSpeed[index]);
                    break;
            }
        }

        /// <summary>
        /// Normal drive
        /// </summary>
        /// <param name="index"></param>
        /// <param name="maxSpeed">max possible speed of the current vehicle</param>
        void Forward(int index, float maxSpeed)
        {
            if (IsInCorrectGear(index))
            {
                //slow down in corners
                if (maxSpeed / _targetSpeed < 1.5)
                {
                    if (math.abs(turnAngle[index]) > 5)
                    {
                        //Debug.Log(1 + math.abs(turnAngle[index]) / maxSteer[index]);
                        //ApplyBrakes(index, 1 + math.abs(turnAngle[index]) / maxSteer[index]);
                        ApplyBrakes(index, 1);
                        //isBraking[index] = true;
                        return;
                    }
                }

                //set speed exactly to max speed if it is close
                float speedDifference = _targetSpeed - maxSpeed;
                if (math.abs(speedDifference) < powerStep[index] || math.abs(speedDifference) < brakeStep[index])
                {
                    _targetSpeed = maxSpeed;
                    return;
                }

                //brake if the vehicle runs faster than the max allowed speed
                if (_targetSpeed > maxSpeed)
                {
                    //for the brake lights to be active only when hard brakes are needed, to avoid short blinking
                    if (speedDifference > 1)
                    {
                        //turn on braking lights
                        isBraking[index] = true;
                    }
                    ApplyBrakes(index, 1);
                    return;
                }
                ApplyAcceleration(index);
            }
            else
            {
                ApplyBrakes(index, 1);
                PutInDrive(index);
            }
        }


        /// <summary>
        /// Go backwards
        /// </summary>
        /// <param name="index"></param>
        void Reverse(int index)
        {
            if (IsInCorrectGear(index))
            {
                ApplyAcceleration(index);
            }
            else
            {
                ApplyBrakes(index, 1);
                PutInReverse(index);
            }
        }


        /// <summary>
        /// Go backwards in opposite direction
        /// </summary>
        /// <param name="index"></param>
        private void AvoidReverse(int index)
        {
            if (IsInCorrectGear(index))
            {
                AvoidBackward();
                Reverse(index);
            }
            else
            {
                StopInDistance(index);
                PutInReverse(index);
            }
        }


        /// <summary>
        /// Stop vehicle immediately
        /// </summary>
        /// <param name="index"></param>
        void StopNow(int index)
        {
            _targetSpeed = 0;
            isBraking[index] = true;
        }


        /// <summary>
        /// Stop the car in a given distance
        /// </summary>
        /// <param name="index"></param>
        private void StopInDistance(int index)
        {
            float stopDistance = math.distance(closestObstacle[index], allBotsPosition[index]) - distanceToStop[index];
            isBraking[index] = true;

            if (stopDistance <= 0)
            {
                StopNow(index);
                return;
            }

            if (currentSpeed <= brakeStep[index])
            {
                StopNow(index);
                return;
            }

            float velocityPerFrame = currentSpeed * fixedDeltaTime;
            int nrOfFrames = (int)(stopDistance / velocityPerFrame) + 1;
            int brakeNrOfFrames = (int)(currentSpeed / brakeStep[index]);
            if (brakeNrOfFrames >= nrOfFrames)
            {
                ApplyBrakes(index, (float)brakeNrOfFrames / nrOfFrames);
            }
        }


        /// <summary>
        /// Stop the vehicle precisely on a waypoint
        /// </summary>
        /// <param name="index"></param>
        void StopInPoint(int index)
        {
            //if there is something in trigger closer -> stop 
            if (!closestObstacle[index].Equals(float3.zero))
            {
                if (math.distance(closestObstacle[index], allBotsPosition[index]) < waypointDistance)
                {
                    StopInDistance(index);
                }
            }

            //stop if the waypoint is behind the vehicle
            if (dotProduct < 0)
            {
                StopNow(index);
                return;
            }

            //compute per frame velocity
            float velocityPerFrame = currentSpeed * fixedDeltaTime;

            //check number of frames required to reach next waypoint
            int nrOfFrames = (int)(waypointDistance / velocityPerFrame);

            if (nrOfFrames < 0)
            {
                nrOfFrames = int.MaxValue;
            }

            //if vehicle is in target -> stop
            if (nrOfFrames == 0)
            {
                StopNow(index);
                return;
            }

            //number of frames required to brake
            int brakeNrOfFrames = (int)(currentSpeed / brakeStep[index]);
            //calculate the required brake power 
            if (brakeNrOfFrames > nrOfFrames)
            {
                ApplyBrakes(index, (float)brakeNrOfFrames / nrOfFrames);
            }
            else
            {
                //if target waypoint is far -> accelerate
                if (nrOfFrames - brakeNrOfFrames > 60)
                {
                    ApplyAcceleration(index);
                    return;
                }
            }
            //turn on the brake lights
            isBraking[index] = true;
        }


        /// <summary>
        /// Opposite direction is required forward 
        /// </summary>
        /// <param name="index"></param>
        void AvoidForward(int index)
        {
            avoidForward = true;
            Forward(index, maxSpeed[index]);
        }


        /// <summary>
        /// Follow the front vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="followSpeed">the speed of the front vehicle</param>
        private void Follow(int index, float followSpeed)
        {
            float distance = math.distance(closestObstacle[index], allBotsPosition[index]) - distanceToStop[index];
            if (distance < 0)
            {
                //distance is dangerously close apply emergency brake
                _targetSpeed = followSpeed - 1;
                return;
            }

            float speedDifference = _targetSpeed - followSpeed;

            //current vehicle moves slower then the vehicle it follows
            if (speedDifference <= 0)
            {
                //speeds are close enough, match them
                if (math.abs(speedDifference) < math.max(powerStep[index], brakeStep[index]))
                {
                    _targetSpeed = followSpeed;
                    return;
                }
                //vehicle needs to accelerate to catch the followed vehicle
                ApplyAcceleration(index);
                return;
            }

            //compute per frame velocity
            float velocityPerFrame = (speedDifference) * fixedDeltaTime;

            //check number of frames required to slow down
            int nrOfFrames = (int)(distance / velocityPerFrame);

            //if nr of frames = 0 => distance is 0, it means that the 2 cars are close enough, set the speed to be equal to the follow speed
            if (nrOfFrames == 0)
            {
                _targetSpeed = followSpeed;
                return;
            }

            //number of frames required to brake
            int brakeNrOfFrames = (int)(speedDifference / brakeStep[index]);

            //calculate the required brake power 
            if (brakeNrOfFrames >= nrOfFrames)
            {
                if (nrOfFrames > 0)
                {
                    ApplyBrakes(index, (float)brakeNrOfFrames / nrOfFrames);
                }
            }
        }
        #endregion


        /// <summary>
        /// Compute the next frame force to be applied to RigidBody
        /// </summary>
        /// <param name="index">current vehicle index</param>
        /// <param name="targetVelocity">target linear speed</param>
        /// <returns></returns>
        private void ComputeBodyForce(int index, float maxSpeed, int gear)
        {
            //set speed limit
            if (maxSpeed == 0 || (math.sign(_targetSpeed * gear) != math.sign(currentSpeed) && math.abs(currentSpeed) > 0.01f))
            {
                _targetSpeed = 0;
            }
            maxSpeed = math.max(_targetSpeed, maxSpeed);
            if (gear == -1)
            {
                if (_targetSpeed < -maxSpeed / 5)
                {
                    _targetSpeed = -maxSpeed / 5;
                }
            }
            else
            {
                if (_targetSpeed > maxSpeed)
                {
                    _targetSpeed = maxSpeed;
                }
            }

            //Debug.Log(maxSpeed * 3.6 + " " + _targetSpeed * 3.6f + " " + currentSpeed * 3.6f);

            float dSpeed = _targetSpeed * gear - currentSpeed;
            float velocity = dSpeed + GetDrag(_targetSpeed, drag[index], fixedDeltaTime);

            //if has trailer
            if (trailerNrOfWheels[index] > 0)
            {
                trailerForce[index] = -trailerRightDirection[index] * Vector3.Dot(trailerVelocity[index], trailerRightDirection[index]) / trailerNrOfWheels[index];

                if (_targetSpeed != 0)
                {
                    velocity += dSpeed * massDifference[index] + GetDrag(_targetSpeed, trailerDrag[index], fixedDeltaTime);
                }
            }

            bodyForce[index] = velocity * forwardDirection[index] / nrOfWheels[index];
        }


        /// <summary>
        /// Check if new waypoint is required
        /// </summary>
        /// <param name="index">current vehicle index</param>
        void ChangeWaypoint(int index)
        {
            if (waypointDistance < minWaypointDistance || (dotProduct < 0 && waypointDistance < minWaypointDistance * 5))
            {
                needsWaypoint[index] = true;
            }
        }


        /// <summary>
        /// Compute the wheel turn amount
        /// </summary>
        /// <param name="index">current vehicle index</param>
        void ComputeWheelRotationAngle(int index)
        {
            wheelRotation[index] += (360 * (currentSpeed / wheelCircumferences[index]) * fixedDeltaTime);
        }


        /// <summary>
        /// Compute the required steering angle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="maxSteer"></param>
        /// <param name="targetVelocity"></param>
        void ComputeSteerAngle(int index, float maxSteer, float targetVelocity)
        {
            float currentTurnAngle = turnAngle[index];
           
            float currentStep = steeringStep[index];
            //increase turn angle with speed
            if (targetVelocity > 14)
            {
                //if speed is greater than 50 km/h increase the turn speed;
                currentStep *= targetVelocity / 14;
            }
            float wheelAngle = SignedAngle(triggerForwardDirection[index], waypointDirection, worldUp);
            //determine the target angle
            if (avoidBackward)
            {
                angle = wheelSign[index] * -maxSteer;
            }
            else
            {
                if (avoidForward)
                {
                    angle = maxSteer;
                }
                else
                {
                    angle = SignedAngle(forwardDirection[index], waypointDirection, worldUp);
                }
            }

            if (!avoidBackward && !avoidForward)
            {
                //if car is stationary, do not turn the wheels
                if (currentSpeed < 1)
                {
                    if (specialDriveAction[index] != TrafficSystem.DriveActions.StopInDistance && specialDriveAction[index] != TrafficSystem.DriveActions.ChangeLane)
                    {
                        angle = 0;
                    }
                }

                //check if the car can turn at current speed         
                float framesToReach = waypointDistance / (targetVelocity * fixedDeltaTime);
                //if it is close to the waypoint turn at normal speed 
                if (framesToReach > 5)
                {
                    //calculate the number of frames required to rotate to the target amount
                    float framesToRotate = math.abs(angle - currentTurnAngle) / currentStep;

                    //car is too fast for this corner
                    //increase the speed turn amount to be able to corner
                    if (framesToRotate > framesToReach + 5)
                    {
                        currentStep *= framesToRotate / framesToReach;
                    }
                    else
                    {
                        //used to straight the wheels after a curve
                        if (math.sign(angle) != math.sign(wheelAngle) && math.abs(angle - wheelAngle) > 10)
                        {
                            currentStep *= framesToRotate / 5;
                        }
                    }
                }
            }
            //apply turning speed
            if (angle - currentTurnAngle < -currentStep)
            {
                currentTurnAngle -= currentStep;
            }
            else
            {
                if (angle - currentTurnAngle > currentStep)
                {
                    currentTurnAngle += currentStep;
                }
                else
                {
                    currentTurnAngle = angle;

                }
            }

            //if the wheel sign and turn angle sign are different the wheel is heading to the destination waypoint, it just needs to keep that direction
            //no additional turning is required so keep the waypoint direction
            if (math.sign(currentTurnAngle) != math.sign(wheelAngle) && math.abs(wheelAngle) < 5)
            {
                currentTurnAngle = angle;
            }

            //clamp the value
            if (currentTurnAngle > maxSteer)
            {
                currentTurnAngle = maxSteer;
            }

            if (currentTurnAngle < -maxSteer)
            {
                currentTurnAngle = -maxSteer;
            }

            //currentTurnAngle = angle;

            //compute the body turn angle based on wheel turn amount
            float turnRadius = wheelDistance[index] / math.tan(math.radians(currentTurnAngle));
            vehicleRotationAngle[index] = (180 * targetVelocity * fixedDeltaTime) / (math.PI * turnRadius) * gear[index];
            turnAngle[index] = currentTurnAngle;
        }


        /// <summary>
        /// Checks if the vehicle can be removed from scene
        /// </summary>
        /// <param name="index">the list index of the vehicle</param>
        void RemoveVehicle(int index)
        {
            bool remove = true;
            for (int i = 0; i < cameraPositions.Length; i++)
            {
                if (math.distancesq(allBotsPosition[index], cameraPositions[i]) < distanceToRemove)
                {
                    remove = false;
                    break;
                }
            }
            readyToRemove[index] = remove;
        }


        /// <summary>
        /// Determine if a car has can change the heading direction
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsInCorrectGear(int index)
        {
            switch (specialDriveAction[index])
            {
                case TrafficSystem.DriveActions.Reverse:
                case TrafficSystem.DriveActions.AvoidReverse:
                    if (gear[index] != -1)
                    {
                        return false;
                    }
                    break;

                default:
                    if (gear[index] != 1)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }


        void PutInDrive(int index)
        {
            if (_targetSpeed == 0)
            {
                if (math.abs(currentSpeed) < 0.0001)
                {
                    gear[index] = 1;
                }
            }
        }


        void PutInReverse(int index)
        {
            if (_targetSpeed == 0)
            {
                if (math.abs(currentSpeed) < 0.0001f)
                {
                    gear[index] = -1;
                }
            }
        }


        /// <summary>
        /// Accelerate current vehicle
        /// </summary>
        /// <param name="index"></param>
        void ApplyAcceleration(int index)
        {
            _targetSpeed += powerStep[index];
        }


        /// <summary>
        /// Brake the vehicle
        /// </summary>
        /// <param name="index"></param>
        /// <param name="power"></param>
        void ApplyBrakes(int index, float power)
        {
            //this is a workaround to mediate the brake power 
            power /= 2;
            if(power<1)
            {
                power = 1;
            }
            _targetSpeed -= brakeStep[index] * power;
            if (_targetSpeed < 0)
            {
                StopNow(index);
            }
        }


        /// <summary>
        /// Opposite direction is required in reverse
        /// </summary>
        void AvoidBackward()
        {
            avoidBackward = true;
        }


        /// <summary>
        /// Compute sign angle between 2 directions
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        float SignedAngle(float3 dir1, float3 dir2, float3 normal)
        {
            if (dir1.Equals(float3.zero))
            {
                return 0;
            }
            dir1 = math.normalize(dir1);
            return math.degrees(math.atan2(math.dot(math.cross(dir1, dir2), normal), math.dot(dir1, dir2)));
        }


        /// <summary>
        /// Compensate the drag from the physics engine
        /// </summary>
        /// <param name="index"></param>
        /// <param name="targetSpeed"></param>
        /// <returns></returns>
        float GetDrag(float targetSpeed, float drag, float fixedDeltaTime)
        {
            float result = targetSpeed / (1 - drag * fixedDeltaTime) - targetSpeed;
            return result;
        }
    }
}
#endif