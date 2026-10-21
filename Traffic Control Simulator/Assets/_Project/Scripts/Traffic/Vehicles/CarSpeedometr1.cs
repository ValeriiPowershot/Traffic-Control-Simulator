using UnityEngine;

[DisallowMultipleComponent]
public class CarSpeedometr1 : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    public float GetCarSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }
}
