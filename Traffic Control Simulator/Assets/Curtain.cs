using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class Curtain : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _duration = 1.3f;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public void CurtainToActiveGameObject(GameObject gameObject)
    {
        StartCoroutine(ActivateCurtain(() => gameObject.SetActive(true)));
    }

    private IEnumerator ActivateCurtain(Action onComplete)
    {
        _canvasGroup.DOFade(1f, _duration).OnComplete(onComplete.Invoke);
        yield return new WaitForSeconds(_duration + 1);
        _canvasGroup.DOFade(0f, _duration);
    }
}
