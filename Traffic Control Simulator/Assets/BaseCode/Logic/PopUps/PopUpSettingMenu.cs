using System;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using BaseCode.Logic.ScriptableObject;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps
{
    public class PopUpSettingMenu : PopUpGameBase
    {
        public ButtonExtension openMainMenu;
        public ButtonExtension helpButton;
        
        public ToggleExtension toggleExtension;
        
        public Slider musicSlider;
        public Slider soundSlider;
        
        private void Start()
        {
            toggleExtension.SetIsOn(GameSettings.isNotificationsOn);
            musicSlider.value = GameSettings.musicVolume;
            soundSlider.value = GameSettings.soundVolume;

            musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);

            helpButton.onClick.AddListener(() =>
            {
                PopUpController.ShowPopUp(PopUpController.GetPopUp<PopUpHelpMenu>());
            });

            openMainMenu.onClick.AddListener(() =>
            {
                PopUpController.HidePopUp(this);
            });
        }

        private void OnMusicSliderValueChanged(float value)
        {
            GameSettings.musicVolume = value;
        }

        private void OnSoundSliderValueChanged(float value)
        {
            GameSettings.soundVolume = value;
        }

        public GameSettings GameSettings => gameManager.saveManager.configSo.gameSettings;
    }
}