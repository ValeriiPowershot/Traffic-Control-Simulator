using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CarouselUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image imageHolder;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Data")]
    [SerializeField] private Sprite[] sprites;

    [Header("Animation")]
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private float slideDistance = 500f;
    [SerializeField] private Dots _dots;

    private int currentIndex;
    private bool isAnimating;

    private void Start()
    {
        imageHolder.sprite = sprites[currentIndex];

        leftButton.onClick.AddListener(Previous);
        rightButton.onClick.AddListener(Next);
    }

    public void Next()
    {
        if (isAnimating) return;

        int nextIndex = (currentIndex + 1) % sprites.Length;
        AnimateChange(nextIndex, 1);

        _dots.ActivateDot(nextIndex);
    }

    public void Previous()
    {
        if (isAnimating) return;

        int prevIndex = (currentIndex - 1 + sprites.Length) % sprites.Length;
        AnimateChange(prevIndex, -1);
    }

    public int GetImagesCount()
    {
        return sprites.Length;
    }

    private void AnimateChange(int newIndex, int direction)
    {
        isAnimating = true;

        Sequence seq = DOTween.Sequence();

        // Уезжаем
        seq.Append(imageHolder.rectTransform
            .DOAnchorPosX(-direction * slideDistance, duration)
            .SetEase(Ease.InOutCubic));

        //seq.Join(imageHolder.DOFade(0, duration));

        seq.AppendCallback(() =>
        {
            imageHolder.sprite = sprites[newIndex];
            imageHolder.rectTransform.anchoredPosition =
                new Vector2(direction * slideDistance, 0);
        });

        // Заезжаем
        seq.Append(imageHolder.rectTransform
            .DOAnchorPosX(0, duration)
            .SetEase(Ease.InCubic));

        //seq.Join(imageHolder.DOFade(1, duration));

        seq.OnComplete(() =>
        {
            currentIndex = newIndex;
            isAnimating = false;
        });
    }
}
