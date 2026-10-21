using UnityEngine;
using BaseCode.Logic.City;

public sealed class CampaignBootstrap : MonoBehaviour
{
    [SerializeField] private TrafficGameSettings settings;
    [SerializeField] private bool menu;
    [SerializeField] private bool launchMenu;
    private void Awake()
    {
        if (launchMenu) return;
        if (menu)
        {
            var screen = gameObject.AddComponent<CampaignMenu>(); screen.Configure(settings);
            return;
        }
        var director = GameManager.ForScene(gameObject.scene);
        if (director == null) director = gameObject.AddComponent<GameManager>();
        director.SetConfiguration(settings);
    }
    private void Start()
    {
        if (!launchMenu) return;
        if (settings != null && UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(settings.MainMenuScenePath) >= 0)
            UnityEngine.SceneManagement.SceneManager.LoadScene(settings.MainMenuScenePath);
        else Debug.LogError("Bootstrap requires settings and an enabled main menu scene.", this);
    }
}
