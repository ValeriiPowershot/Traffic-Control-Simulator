using BaseCode.Infrastructure.Factory;
using Infrastructure;
using Infrastructure.AssetManagment;
using Infrastructure.States;
using Services;

namespace BaseCode.Infrastructure.States
{
  public class BootstrapState : IState
  {
    private const string BootstrapScene = "BootstrapScene";
    private readonly GameStateMachine _stateMachine;
    private readonly SceneLoader _sceneLoader;
    private readonly AllServices _services;
      
    public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services)
    {
      _stateMachine = stateMachine;
      _sceneLoader = sceneLoader;
      _services = services;
      
      RegisterServices();
    }

    public void Enter() =>
      _sceneLoader.Load(BootstrapScene, onLoaded: EnterLoadLevel);

    public void Exit()
    {
    }
    
    private void RegisterServices()
    {
      _services.RegisterSingle<IGameStateMachine>(_stateMachine);
      _services.RegisterSingle<IAssetsProvider>(new AssetsProvider());
      _services.RegisterSingle<IGameFactory>(new GameFactory(_services.Single<IAssetsProvider>())); 
    }
    
    private void EnterLoadLevel() =>
      _stateMachine.Enter<LoadLevelState, string>(SelectSceneData.SelectedSceneName);
  }
}