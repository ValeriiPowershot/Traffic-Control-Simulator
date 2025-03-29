using System;
using BaseCode.Logic;
using BaseCode.Logic.ScriptableObject;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BaseCode.Extensions.UI
{
    public static class TextMeshProUGUIExtension
    {
        public static void Toggle(this TextMeshProUGUI textMeshProUGUI)
        {
            textMeshProUGUI.gameObject.SetActive(!textMeshProUGUI.isActiveAndEnabled);
        }
        public static void SetText(this TextMeshProUGUI textMeshProUGUI, string text)
        {
            textMeshProUGUI.text = text;
        }
    }
} 