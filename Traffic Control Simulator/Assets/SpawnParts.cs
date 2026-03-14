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

    [Header("Animation")]
    [SerializeField] private float scaleDuration = 0.3f;
    [SerializeField] private float dropDuration = 0.25f;
    [SerializeField] private float dropDistance = 0.25f;

    public event Action OnSpawnEnd;

    private Vector3[] partsScale;
    private Vector3[] partsPosition;

    private void Awake()
    {
        partsScale = new Vector3[parts.Length];
        partsPosition = new Vector3[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] == null) continue;

            partsScale[i] = parts[i].transform.localScale;
            partsPosition[i] = parts[i].transform.position;

            parts[i].SetActive(false);
        }
    }

    public void StartSpawn()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        Debug.Log(gameObject.name + " spawn routine");

        int triggerIndex = Mathf.CeilToInt(parts.Length * 0.33f);
        bool eventCalled = false;

        for (int i = 0; i < parts.Length; i++)
        {
            GameObject part = parts[i];
            if (part == null) continue;

            part.SetActive(true);

            Transform t = part.transform;

            Vector3 finalPos = partsPosition[i];
            Vector3 startPos = finalPos + Vector3.up * dropDistance;

            t.position = startPos;
            t.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            seq.Append(t.DOScale(partsScale[i], scaleDuration).SetEase(Ease.OutBack));
            seq.Join(t.DOMove(finalPos, dropDuration).SetEase(Ease.OutBounce));

            // вызываем событие на 2/3
            if (!eventCalled && i >= triggerIndex)
            {
                eventCalled = true;
                OnSpawnEnd?.Invoke();
            }

            yield return new WaitForSeconds(delayBetween);
        }

        // если вдруг массив очень маленький
        if (!eventCalled)
        {
            OnSpawnEnd?.Invoke();
        }

        // запускаем следующие SpawnParts
        foreach (var next in nextParts)
        {
            if (next != null)
                next.StartSpawn();
        }
    }
}
