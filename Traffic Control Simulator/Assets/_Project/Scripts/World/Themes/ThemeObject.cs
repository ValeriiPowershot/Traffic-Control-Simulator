using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteAlways] // Позволяет скрипту работать в режиме редактирования (вне игры)
public class ThemeObject : MonoBehaviour
{
    [System.Serializable]
    public class ObjectTheme
    {
        public Material[] materials;
    }

    [Header("Компоненты")]
    public Renderer targetRenderer;

    [Header("Список тем для этого объекта")]
    public List<ObjectTheme> objectThemes = new();

    // Храним текущий индекс, чтобы объект знал свою текущую тему
    [HideInInspector] public int currentThemeIndex = 0;

    private void Reset()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
    }

    // Вызывается автоматически при любом изменении значений в инспекторе
    private void OnValidate()
    {
        targetRenderer = GetComponent<Renderer>();

        // Сразу обновляем внешний вид при редактировании материалов в инспекторе
        ApplyTheme(currentThemeIndex);
    }

    /// <summary>
    /// Применяет массив материалов из выбранной темы
    /// </summary>
    public void ApplyTheme(int themeIndex)
    {
        if (targetRenderer == null) return;

        currentThemeIndex = themeIndex;

        if (themeIndex >= 0 && themeIndex < objectThemes.Count)
        {
            ObjectTheme selectedTheme = objectThemes[themeIndex];

            if (selectedTheme.materials != null && selectedTheme.materials.Length > 0)
            {
                // Используем sharedMaterials вместо materials,
                // чтобы избежать утечки памяти при изменении в редакторе вне игры
                targetRenderer.sharedMaterials = selectedTheme.materials;
            }
        }
    }
}
