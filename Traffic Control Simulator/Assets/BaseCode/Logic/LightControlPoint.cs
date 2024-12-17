using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private void OnTriggerEnter(Collider Other)
    {
        if (Other.TryGetComponent<ICar>(out ICar CollidedCar))
        {
            if(_pointType == LightPointType.Entry)
                _parentLight.AddNewCar(CollidedCar);

            else if(_pointType == LightPointType.Exit)
                _parentLight.RemoveCar(CollidedCar);
        }
    }
}

public enum LightPointType
{
    None,
    Entry,
    Exit,
}
