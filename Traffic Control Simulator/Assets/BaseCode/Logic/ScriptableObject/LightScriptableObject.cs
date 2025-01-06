using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    [CreateAssetMenu(fileName = "LightSo", menuName = "So/Light", order = 0)]
    public class LightScriptableObject : UnityEngine.ScriptableObject
    {
        [SerializeField] private float switchDelay; // 0.3

        public float SwitchDelay
        {
            get => switchDelay;
            set => switchDelay = value;
        }
        
    }
}