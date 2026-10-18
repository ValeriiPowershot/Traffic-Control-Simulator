using System;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [Header("Список всех объектов темы")]
    public List<ThemeObject> themeObjects = new();

    /// <summary>
    /// Переключить тему по индексу для всех объектов
    /// </summary>
    public void ChangeTheme(int themeIndex)
    {
        foreach (var obj in themeObjects)
        {
            if (obj != null)
            {
                obj.ApplyTheme(themeIndex);
            }
        }
    }

    private void Update()
    {
        // Тест: кнопки 1, 2, 3 на клавиатуре
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) ChangeTheme(0);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) ChangeTheme(1);
        if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) ChangeTheme(2);
    }

    [ContextMenu("Найти все ThemeObject на сцене")]
    private void FindAllObjectsInScene()
    {
        themeObjects = new List<ThemeObject>(
            FindObjectsByType<ThemeObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None)
        );

        Debug.Log($"Автоматически добавлено объектов: {themeObjects.Count}");
    }
}
