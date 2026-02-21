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
            // 1. Обновляем waypoint и тормоз/движение
            waypointSystem.UpdateWaypoints(transform, movement);

            // 2. Обновляем рулевое управление
            steering.UpdateSteering(transform, waypointSystem.CurrentTarget, movement.IsBraking);

            // 3. Применяем движение
            movement.UpdateMovement();

            // 4. Если впереди есть объект, CarDetection уже вызвала SetBrake
            //    — просто оставляем торможение как есть, не блокируем остальные функции
        }
    }
}
