using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DropdownOption
{
    public string text;
    public Sprite icon;
}

public class ImageDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Image selectedImage;

    public DropdownOption[] options;

    void Start()
    {
        dropdown.ClearOptions();

        var list = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();

        foreach (var option in options)
        {
            list.Add(new TMP_Dropdown.OptionData(option.text, option.icon));
        }

        dropdown.AddOptions(list);

        dropdown.onValueChanged.AddListener(OnChange);
        OnChange(0);
    }

    void OnChange(int index)
    {
        selectedImage.sprite = options[index].icon;
    }
}
