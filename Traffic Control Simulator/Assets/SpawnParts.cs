using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpawnParts : MonoBehaviour
{
    [SerializeField] private GameObject[] parts;

    [Header("Next Spawn Parts")]
    [SerializeField] private SpawnParts[] nextParts;

    [Header("Timing")]
    [SerializeField] private float delayBetween = 0.2f;

    [SerializeField] private bool startOnAwake;

    [Header("Animation")]
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float dropDuration = 0.25f;
    [SerializeField] private float dropDistance = 0.25f;

    public event Action OnSpawnEnd;

    private Vector3[] partsScale;
    private Vector3[] partsPosition;

    private Coroutine spawnRoutine;

    private void Awake()
    {
        CacheParts();
    }

    private void CacheParts()
    {
        if (parts == null)
            return;

        partsScale = new Vector3[parts.Length];
        partsPosition = new Vector3[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == null)
                continue;

            Transform t = parts[i].transform;

            partsScale[i] = t.localScale;
            partsPosition[i] = t.localPosition;

            parts[i].SetActive(false);
        }
    }

    public void StartSpawn()
    {
        StopAllCoroutines();
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void ResetParts()
    {
        StopAllCoroutines();

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == null)
                continue;

            Transform t = parts[i].transform;

            t.DOKill();

            // защита от выхода за массив
            if (partsPosition != null && i < partsPosition.Length)
                t.localPosition = partsPosition[i];

            if (partsScale != null && i < partsScale.Length)
                t.localScale = partsScale[i];

            parts[i].SetActive(false);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        int triggerIndex = Mathf.CeilToInt(parts.Length * 0.33f);
        bool eventCalled = false;

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == null)
                continue;

            parts[i].SetActive(true);

            Transform t = parts[i].transform;

            t.DOKill();

            Vector3 finalPos = partsPosition != null && i < partsPosition.Length
                ? partsPosition[i]
                : t.localPosition;

            Vector3 startPos = finalPos + Vector3.up * dropDistance;

            t.localPosition = startPos;
            t.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            seq.Append(
                t.DOScale(partsScale[i], scaleDuration)
                    .SetEase(Ease.OutBack)
            );

            seq.Join(
                t.DOLocalMove(finalPos, dropDuration)
                    .SetEase(Ease.OutBounce)
            );

            if (!eventCalled && i >= triggerIndex)
            {
                eventCalled = true;
                OnSpawnEnd?.Invoke();
            }

            yield return new WaitForSeconds(delayBetween);
        }

        if (!eventCalled)
            OnSpawnEnd?.Invoke();

        foreach (var next in nextParts)
        {
            if (next != null)
                next.StartSpawn();
        }
    }
}
