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
    [SerializeField] private Curtain curtain;

    [Header("Animation")]
    [SerializeField] private float moveDistance = 15f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private int currentIndex = 0;
    private Sequence currentSequence;

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
        if (levels.Length <= 1)
            return;

        int nextIndex = currentIndex + 1;

        if (nextIndex >= levels.Length)
            nextIndex = 0;

        ChangeLevel(nextIndex, true);
    }

    public void PreviousLevel()
    {
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
        if (newIndex == currentIndex)
            return;

        SpawnPartHolder currentHolder = levels[currentIndex].holder;
        SpawnPartHolder nextHolder = levels[newIndex].holder;

        if (currentHolder == null || nextHolder == null)
            return;

        GameObject current = currentHolder.gameObject;
        GameObject next = nextHolder.gameObject;

        // убиваем прошлую анимацию
        currentSequence?.Kill();

        current.transform.DOKill();
        next.transform.DOKill();

        float direction = moveLeft ? -1f : 1f;

        Vector3 centerPos = centerPoint.position;

        Vector3 nextStartPos =
            centerPos + new Vector3(-direction * moveDistance, 0f, 0f);

        Vector3 currentEndPos =
            centerPos + new Vector3(direction * moveDistance, 0f, 0f);

        // подготавливаем следующий уровень
        nextHolder.TurnOffAllParts();

        next.transform.position = nextStartPos;
        next.SetActive(true);

        currentSequence = DOTween.Sequence();

        // текущий уезжает
        currentSequence.Append(
            current.transform.DOMove(currentEndPos, duration)
                .SetEase(ease)
        );

        // следующий приезжает
        currentSequence.Join(
            next.transform.DOMove(centerPos, duration)
                .SetEase(ease)
        );

        // индекс обновляем сразу
        currentIndex = newIndex;

        currentSequence.OnComplete(() =>
        {
            current.SetActive(false);
            currentHolder.TurnOffAllParts();

            nextHolder.RestartParts();
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
