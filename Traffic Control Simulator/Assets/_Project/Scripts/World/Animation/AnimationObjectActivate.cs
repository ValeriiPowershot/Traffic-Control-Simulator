using UnityEngine;

[DisallowMultipleComponent]
public class AnimationObjectActivate : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    //Animation Action
    public void Activate() =>
        _target.SetActive(true);

    //Animation Action
    public void Deactivate() =>
        _target.SetActive(false);
}
