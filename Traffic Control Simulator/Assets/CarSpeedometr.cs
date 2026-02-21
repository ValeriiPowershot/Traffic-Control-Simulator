using UnityEngine;

[DisallowMultipleComponent]
public class CarSpeedometr : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    public float GetCarSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }
}
