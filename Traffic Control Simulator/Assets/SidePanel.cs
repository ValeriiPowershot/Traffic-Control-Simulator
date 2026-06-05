using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class SidePanel : MonoBehaviour
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Settings")]
    [SerializeField] private Direction direction = Direction.Left;
    [SerializeField] private float offset = 500f;
    [SerializeField] private float duration = 0.3f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Vector2 openedPosition;
    private Vector2 closedPosition;

    private bool isOpened = true;
    private Tween currentTween;

    private void Awake()
    {
        if (target == null)
        {
            Debug.LogError($"{name}: Target is not assigned!");
            enabled = false;
            return;
        }

        openedPosition = target.anchoredPosition;
        closedPosition = openedPosition + GetOffset();
    }

    private Vector2 GetOffset()
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector2.left * offset;

            case Direction.Right:
                return Vector2.right * offset;

            case Direction.Up:
                return Vector2.up * offset;

            case Direction.Down:
                return Vector2.down * offset;

            default:
                return Vector2.zero;
        }
    }

    public void Open()
    {
        currentTween?.Kill();

        currentTween = target
            .DOAnchorPos(openedPosition, duration)
            .SetEase(ease);

        isOpened = true;
    }

    public void Close()
    {
        currentTween?.Kill();

        currentTween = target
            .DOAnchorPos(closedPosition, duration)
            .SetEase(ease);

        isOpened = false;
    }

    public void Toggle()
    {
        if (isOpened)
            Close();
        else
            Open();
    }
}
