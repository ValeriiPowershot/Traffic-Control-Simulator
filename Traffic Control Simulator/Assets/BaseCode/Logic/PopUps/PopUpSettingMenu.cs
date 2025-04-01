using System;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.PopUp_Base;
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
                PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpHelpMenu>());
            });

            openMainMenu.onClick.AddListener(() =>
            {
                PopUpManager.HidePopUp(this);
            });
        }

        private void OnMusicSliderValueChanged(float value)
        {
            GameSettings.musicVolume = value;
            GameManager.vfxManager.mainGameMusic.volume = value;
        }

        private void OnSoundSliderValueChanged(float value)
        {
            GameSettings.soundVolume = value;
        }

        public GameSettings GameSettings => GameManager.saveManager.configSo.gameSettings;
    }
}