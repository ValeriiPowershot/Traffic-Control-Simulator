using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    [System.Serializable]
    public class LanguageData
    {
        public string languageName;
        public Sprite flag;
    }

    [Header("UI")]
    [SerializeField] private Image flagImage;
    [SerializeField] private TMP_Text languageText;

    [Header("Languages")]
    [SerializeField] private List<LanguageData> languages = new();

    private int currentIndex;

    private void Start()
    {
        UpdateUI();
    }

    public void NextLanguage()
    {
        if (languages.Count == 0)
            return;

        currentIndex++;

        if (currentIndex >= languages.Count)
            currentIndex = 0;

        UpdateUI();
    }

    public void PreviousLanguage()
    {
        if (languages.Count == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = languages.Count - 1;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (languages.Count == 0)
            return;

        var language = languages[currentIndex];

        flagImage.sprite = language.flag;
        languageText.text = language.languageName;
    }

    public string GetCurrentLanguage()
    {
        return languages[currentIndex].languageName;
    }
}
