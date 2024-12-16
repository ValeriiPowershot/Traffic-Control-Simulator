using UnityEngine;

namespace Infrastructure
{
  public class GameRunner : MonoBehaviour
  {
    public GameBootstrapper BootstrapperPrefab;
    
    private void Awake()
    {
      GameBootstrapper bootstrapper = FindObjectOfType<GameBootstrapper>();
      
      if(bootstrapper != null) return;

      Instantiate(BootstrapperPrefab);
    }
  }
}