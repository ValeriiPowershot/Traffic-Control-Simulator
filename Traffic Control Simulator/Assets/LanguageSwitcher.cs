using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguageSwitcher : MonoBehaviour
{
    public string[] languages;

    private int currentIndex = 0;

    public void NextLanguage()
    {
        if (languages == null || languages.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % languages.Length;
        StartCoroutine(SetLanguage(languages[currentIndex]));
    }

    public void PreviousLanguage()
    {
        if (languages == null || languages.Length == 0)
            return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = languages.Length - 1;

        StartCoroutine(SetLanguage(languages[currentIndex]));
    }

    private IEnumerator SetLanguage(string code)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;

        for (int i = 0; i < locales.Count; i++)
        {
            if (locales[i].Identifier.Code == code)
            {
                LocalizationSettings.SelectedLocale = locales[i];
                yield break;
            }
        }

        Debug.LogWarning("Locale not found: " + code);
    }
}
