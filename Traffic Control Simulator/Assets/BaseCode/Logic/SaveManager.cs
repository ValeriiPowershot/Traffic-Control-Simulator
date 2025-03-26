using BaseCode.Logic.ScriptableObject;
using UnityEngine;

namespace BaseCode.Logic
{
    public class SaveManager : SingletonManagerBase<SaveManager>
    {
        public PlayerSo playerSo;
        public ConfigSo configSo;
        public SceneSo sceneSo;
    }
    
}