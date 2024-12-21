using UnityEngine;

namespace BaseCode.Logic.Lights
{
    //Marks all enter and exit points for space controlled by parent light
    public class LightControlPoint : MonoBehaviour
    {
        [SerializeField] private LightPointType _pointType;
        private BasicLight _parentLight;

        //These objects should always be child for light they using
        private void Awake()
        {
            _parentLight = GetComponentInParent<BasicLight>();
        }

        //collider on this object should include interactions only with car layer
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Sex");
            if (other.TryGetComponent(out ICar collidedCar))
            {
                _parentLight.AddNewCar(collidedCar);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ICar collidedCar))
            {
                _parentLight.RemoveCar(collidedCar);
                collidedCar.PassLightState(LightState.None); 
            }
        }
    }

    public enum LightPointType
    {
        None,
        Entry,
        Exit,
    }
}