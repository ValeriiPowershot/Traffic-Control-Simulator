using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    
    public enum FxTypes
    {
        Bubble,
        Happy,
        DiabolicalLaugh,
        Angry,
        Neutral,
        StarCarCrash,
        Smoke
    }
    
    [CreateAssetMenu(fileName = "FxSo", menuName = "So/FxSo", order = 0)]
    public class FxEffectsScriptableObject : UnityEngine.ScriptableObject
    {
        public List<FxEffect> fxEffects = new List<FxEffect>();
        
        public GameObject GetFxPrefab(FxTypes fxType)
        {
            foreach (var fxEffect in fxEffects)
            {
                if (fxEffect.fxType == fxType)
                {
                    return fxEffect.fxPrefab;
                }
            }
            return null;
        }
        
    }
    
    [Serializable]
    public class FxEffect
    {
        public FxTypes fxType;
        public GameObject fxPrefab;
    }
}