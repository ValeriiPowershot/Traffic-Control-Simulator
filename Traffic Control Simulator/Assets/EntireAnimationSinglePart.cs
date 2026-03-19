using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class EntireAnimationSinglePart : MonoBehaviour
{
    [SerializeField] private Transform part;

    [Header("Timing")]
    [SerializeField] private float startDelay = 1f;

    [Header("Animation")]
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float dropDuration = 0.25f;
    [SerializeField] private float dropDistance = 0.25f;

    private Vector3 _startScale;
    private Vector3 finalPosition;

    private void Start()
    {
        finalPosition = part.position;
        _startScale = part.localScale;

        // стартовое состояние
        part.localScale = Vector3.zero;
        part.position = finalPosition + Vector3.up * dropDistance;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        Sequence seq = DOTween.Sequence();

        seq.Append(part.DOScale(_startScale, scaleDuration).SetEase(Ease.OutBack));
        seq.Join(part.DOMoveY(finalPosition.y, dropDuration).SetEase(Ease.OutBounce));
    }
}
