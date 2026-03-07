using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class DropdownIcons : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Sprite[] icons;

    void Start()
    {
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var icon in icons)
        {
            options.Add(new TMP_Dropdown.OptionData("", icon));
        }

        dropdown.AddOptions(options);
    }
}
