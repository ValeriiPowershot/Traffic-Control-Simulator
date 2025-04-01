using BaseCode.Logic.ScriptableObject;

namespace BaseCode.Logic.Managers
{
    public class SaveManager : SingletonManagerBase<SaveManager>
    {
        public PlayerSo playerSo;
        public ConfigSo configSo;
        public SceneSo sceneSo;
    }
    
}