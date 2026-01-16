using UnityEngine;

namespace CarControllerScripts
{
    public class CarController : MonoBehaviour
    {
        public CarMovement movement;
        public CarSteering steering;
        public WaypointSystem waypointSystem;

        void Start()
        {
            movement.Initialize(GetComponent<Rigidbody>());
        }

        void FixedUpdate()
        {
            // Обновляем waypoint и тормоз/движение
            waypointSystem.UpdateWaypoints(transform, movement);

            // Обновляем рулевое управление
            steering.UpdateSteering(transform, waypointSystem.CurrentTarget, movement.IsBraking);

            // Применяем движение
            movement.UpdateMovement();
        }
    }
}
