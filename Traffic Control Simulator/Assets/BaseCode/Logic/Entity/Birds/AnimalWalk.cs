using DG.Tweening;
using UnityEngine;

namespace BaseCode.Logic.Entity.Birds
{
    public class AnimalWalk : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _animatorSpeed;
        
        [SerializeField] private float _speed = 1f; 
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _pauseDuration = 1f;

        private Vector3 _startPosition; 
        private bool _movingForward = true;

        private void Start()
        {
            _startPosition = transform.position;
            _animator.speed = _animatorSpeed;
            Move();
        }

        private void Move()
        {
            Vector3 targetPosition = _movingForward
                ? _startPosition + transform.forward * _distance
                : _startPosition;

            transform.DOMove(targetPosition, _distance / _speed)
                .SetEase(Ease.Linear)
                .OnComplete(() => 
                {
                    _animator.SetBool("IsPaused", true);
                    DOVirtual.DelayedCall(_pauseDuration, TurnAround);
                });
        }

        private void TurnAround()
        {
            transform.DORotate(transform.eulerAngles + new Vector3(0, 180, 0), 0.5f)
                .OnComplete(() =>
                {
                    _movingForward = !_movingForward;
                    _animator.SetBool("IsPaused", false);
                    Move();
                });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Vector3 startPosition = Application.isPlaying ? _startPosition : transform.position;
            Vector3 targetPosition = startPosition + transform.forward * _distance;

            Gizmos.DrawLine(startPosition, targetPosition);

            Gizmos.DrawSphere(startPosition, 0.1f);
            Gizmos.DrawSphere(targetPosition, 0.1f);
        }
    }
}
