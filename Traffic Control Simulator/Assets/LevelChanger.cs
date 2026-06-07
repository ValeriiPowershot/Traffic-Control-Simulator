using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class LevelChanger : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string sceneName;
        public string levelName;
        public SpawnPartHolder holder;
    }

    [Header("Levels")]
    [SerializeField] private LevelData[] levels;

    [Header("UI")]
    [SerializeField] private TMP_Text levelNameText;

    [Header("References")]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Curtain curtain;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    [Header("Events")]
    public UnityEvent<int> onLevelChanged;

    private int currentIndex = 0;
    private bool isChangingLevel;
    private Sequence sequence;

    private void Start()
    {
        InitializeLevels();
        UpdateUI();

        onLevelChanged?.Invoke(currentIndex);
    }

    // ---------------- INPUT ----------------

    public void NextLevel()
    {
        if (isChangingLevel || levels.Length <= 1)
            return;

        int nextIndex = (currentIndex + 1) % levels.Length;
        ChangeLevel(nextIndex, true);
    }

    public void PreviousLevel()
    {
        if (isChangingLevel || levels.Length <= 1)
            return;

        int nextIndex = (currentIndex - 1 + levels.Length) % levels.Length;
        ChangeLevel(nextIndex, false);
    }

    // ---------------- CORE ----------------

    private void ChangeLevel(int newIndex, bool moveLeft)
    {
        if (isChangingLevel || newIndex == currentIndex)
            return;

        var currentHolder = levels[currentIndex].holder;
        var nextHolder = levels[newIndex].holder;

        if (currentHolder == null || nextHolder == null)
            return;

        isChangingLevel = true;

        sequence?.Kill();
        currentHolder.transform.DOKill();
        nextHolder.transform.DOKill();

        Vector3 currentEndPos = moveLeft ? leftPoint.position : rightPoint.position;
        Vector3 nextStartPos = moveLeft ? rightPoint.position : leftPoint.position;
        Vector3 centerPos = centerPoint.position;

        nextHolder.TurnOffAllParts();

        nextHolder.transform.position = nextStartPos;
        nextHolder.gameObject.SetActive(true);

        sequence = DOTween.Sequence();

        sequence.Append(
            currentHolder.transform.DOMove(currentEndPos, duration)
                .SetEase(ease)
        );

        sequence.Join(
            nextHolder.transform.DOMove(centerPos, duration)
                .SetEase(ease)
        );

        sequence.OnComplete(() =>
        {
            currentHolder.gameObject.SetActive(false);
            currentHolder.TurnOffAllParts();

            nextHolder.transform.position = centerPos;
            nextHolder.RestartParts();

            currentIndex = newIndex;

            UpdateUI();

            // 🔥 ВАЖНО: ЕДИНЫЙ источник обновления
            onLevelChanged?.Invoke(currentIndex);

            isChangingLevel = false;
        });
    }

    // ---------------- UI ----------------

    private void UpdateUI()
    {
        if (levelNameText != null && levels.Length > 0)
            levelNameText.text = levels[currentIndex].levelName;
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
                levels[i].holder.transform.position = centerPoint.position;
        }
    }

    // ---------------- PUBLIC ----------------

    public int LevelCount() => levels.Length;

    public int CurrentIndex() => currentIndex;
}
