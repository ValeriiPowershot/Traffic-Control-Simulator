using UnityEngine;

public class Car : MonoBehaviour
{
    private float _speed;

    public void Init(float speed)
    {
        _speed = speed;
    }

    private void Update()
    {
        //transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
