using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Curtain : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _duration = 1.3f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // После загрузки новой сцены открыть занавес
        _canvasGroup.DOFade(0f, _duration);
    }

    public void LoadSceneWithCurtain(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Закрываем занавес
        yield return _canvasGroup.DOFade(1f, _duration).WaitForCompletion();

        // Загружаем сцену
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Открытие произойдет автоматически в OnSceneLoaded
    }
}
