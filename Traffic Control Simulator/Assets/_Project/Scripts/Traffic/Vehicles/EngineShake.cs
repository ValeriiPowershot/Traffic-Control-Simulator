using DG.Tweening;
using UnityEngine;

public class EngineShake : MonoBehaviour
{
    [Header("Tuning")]
    public float strength = 0.03f;   // сила тряски
    public float randomness = 10f;   // хаотичность (чуть меньше для плотности)
    public int vibrato = 20;         // высокая частота

    private Tween shakeTween;

    void OnEnable()
    {
        StartShake();
    }

    void OnDisable()
    {
        shakeTween?.Kill();
    }

    public void StartShake()
    {
        shakeTween?.Kill();

        shakeTween = transform.DOShakePosition(
                duration: 5f, // длинный цикл = нет заметных пауз
                strength: new Vector3(strength, strength * 0.4f, strength * 0.7f),
                vibrato: vibrato,
                randomness: randomness,
                fadeOut: false
            )
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
