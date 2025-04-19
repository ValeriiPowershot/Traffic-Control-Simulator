using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BlinkingTextDoTween : MonoBehaviour
{
    [Header("Blink Settings")]
    [Tooltip("Minimum alpha (transparency)")]
    [Range(0f, 1f)] public float MinAlpha = 0.2f;

    [Tooltip("Maximum alpha (transparency)")]
    [Range(0f, 1f)] public float MaxAlpha = 1f;

    [Tooltip("Duration of one cycle (in seconds)")]
    public float Duration = 1f;

    [Tooltip("Pause duration between blinks (in seconds)")]
    public float LoopPause = 0.2f;

    public Ease BlinkEase;

    [Tooltip("Automatically play on start")]
    public bool PlayOnStart = true;

    private TextMeshProUGUI _textComponent;
    private Sequence _blinkSequence;

    private void Awake() =>
        _textComponent = GetComponent<TextMeshProUGUI>();

    private void Start()
    {
        if (PlayOnStart)
            StartBlinking();
    }

    private void StartBlinking()
    {
        _blinkSequence?.Kill();

        Color color = _textComponent.color;
        color.a = MaxAlpha;
        _textComponent.color = color;

        _blinkSequence = DOTween.Sequence();
        _blinkSequence.Append(_textComponent.DOFade(MinAlpha, Duration / 2f).SetEase(BlinkEase));
        _blinkSequence.Append(_textComponent.DOFade(MaxAlpha, Duration / 2f).SetEase(BlinkEase));
        _blinkSequence.AppendInterval(LoopPause);
        _blinkSequence.SetLoops(-1);
    }

    private void StopBlinking()
    {
        _blinkSequence?.Kill();

        Color color = _textComponent.color;
        color.a = MaxAlpha;
        _textComponent.color = color;
    }

    private void OnDisable() =>
        StopBlinking();
}
