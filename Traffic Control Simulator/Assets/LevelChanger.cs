using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;

[DisallowMultipleComponent]
public class LevelChanger : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string sceneName;
        public SpawnPartHolder holder;
    }

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    [Header("References")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Curtain curtain;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private int currentIndex = 0;
    private Sequence currentSequence;
    private bool isChangingLevel;

    private void Start()
    {
        InitializeLevels();

        if (levels != null &&
            levels.Length > 0 &&
            levels[0].holder != null)
        {
            levels[0].holder.RestartParts();
        }
    }

    private void InitializeLevels()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].holder == null)
                continue;

            bool isCurrent = i == currentIndex;

            levels[i].holder.gameObject.SetActive(isCurrent);

            if (isCurrent)
            {
                levels[i].holder.transform.position = centerPoint.position;
            }
        }
    }

    public void NextLevel()
    {
        if (isChangingLevel)
            return;

        if (levels.Length <= 1)
            return;

        int nextIndex = currentIndex + 1;

        if (nextIndex >= levels.Length)
            nextIndex = 0;

        ChangeLevel(nextIndex, true);
    }

    public void PreviousLevel()
    {
        if (isChangingLevel)
            return;

        if (levels.Length <= 1)
            return;

        int nextIndex = currentIndex - 1;

        if (nextIndex < 0)
            nextIndex = levels.Length - 1;

        ChangeLevel(nextIndex, false);
    }

    public void PlayCurrentLevel()
    {
        if (levels == null || levels.Length == 0)
            return;

        string sceneName = levels[currentIndex].sceneName;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"Scene name is empty for level index: {currentIndex}");
            return;
        }

        if (curtain == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        StartCoroutine(LoadSceneWithCurtain(sceneName));
    }

    private IEnumerator LoadSceneWithCurtain(string sceneName)
    {
        CanvasGroup canvasGroup = curtain.GetComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = true;

        yield return canvasGroup
            .DOFade(1f, 1.3f)
            .SetEase(Ease.OutCubic)
            .WaitForCompletion();

        SceneManager.LoadScene(sceneName);
    }

    private void ChangeLevel(int newIndex, bool moveLeft)
    {
        if (isChangingLevel)
            return;

        if (newIndex == currentIndex)
            return;

        if (leftPoint == null || rightPoint == null || centerPoint == null)
        {
            Debug.LogError(
                "Пожалуйста, назначьте Center Point, Left Point и Right Point в инспекторе!",
                this
            );
            return;
        }

        SpawnPartHolder currentHolder = levels[currentIndex].holder;
        SpawnPartHolder nextHolder = levels[newIndex].holder;

        if (currentHolder == null || nextHolder == null)
            return;

        isChangingLevel = true;

        GameObject current = currentHolder.gameObject;
        GameObject next = nextHolder.gameObject;

        currentSequence?.Kill();

        current.transform.DOKill();
        next.transform.DOKill();

        Vector3 currentEndPos = moveLeft ? leftPoint.position : rightPoint.position;
        Vector3 nextStartPos = moveLeft ? rightPoint.position : leftPoint.position;
        Vector3 centerPos = centerPoint.position;

        nextHolder.TurnOffAllParts();

        next.transform.position = nextStartPos;
        next.SetActive(true);

        currentSequence = DOTween.Sequence();

        currentSequence.Append(
            current.transform.DOMove(currentEndPos, duration)
                .SetEase(ease)
        );

        currentSequence.Join(
            next.transform.DOMove(centerPos, duration)
                .SetEase(ease)
        );

        currentIndex = newIndex;

        currentSequence.OnComplete(() =>
        {
            current.SetActive(false);
            currentHolder.TurnOffAllParts();

            next.transform.position = centerPos;

            nextHolder.RestartParts();

            isChangingLevel = false;
        });
    }

    public int LevelCount()
    {
        return levels.Length;
    }

    public int CurrentIndex()
    {
        return currentIndex;
    }
}
