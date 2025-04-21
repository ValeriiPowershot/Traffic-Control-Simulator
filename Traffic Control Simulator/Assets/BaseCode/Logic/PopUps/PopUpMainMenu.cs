using BaseCode.Extensions;
using BaseCode.Extensions.UI;
using BaseCode.Logic.Managers;
using BaseCode.Logic.PopUps.PopUp_Base;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic.PopUps
{
    public class PopUpMainMenu : PopUpGameBase
    {
        [SerializeField] private ButtonExtension _tapToStartButton;
        [SerializeField] private GameObject _tapToStartText;

        private readonly IteratorRefExtension<float> _lastMusicTime = new(); // move to fx manager?
        
        private void Start() =>
            _tapToStartButton.onClick.AddListener(OnTapToStartButtonClicked);

        public override void OnStartShow()
        {
            base.OnStartShow();
            GameManager.vfxManager.PlayGameMusic(VfxTypes.GameMenuPopUpVfx, _lastMusicTime.Value);
        }
        public override void OnStartHidden()
        {
            base.OnStartHidden();
            GameManager.StartCoroutine(GameManager.vfxManager.FadeOutMusic(_lastMusicTime));
        }
        private void OnTapToStartButtonClicked()
        {
            //GameManager.StartCoroutine(GameManager.vfxManager.FadeOutMusic(_lastMusicTime));
            PopUpManager.ShowPopUpFromBase(PopUpManager.GetPopUp<PopUpLevelsMenu>());
            PopUpManager.HidePopUp(PopUpManager.GetPopUp<PopUpMainMenu>());
            //SceneLoadManager.LoadSceneNormal(SceneID.Levels);
        }

        private SceneLoadManager SceneLoadManager => GameManager.sceneLoadManager;
    }
}