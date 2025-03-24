using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseCode.Logic.ScriptableObject
{
    
    public enum VfxTypes
    {
        ButtonClickVfx,
        GameMenuPopUpVfx
    }
    
    [CreateAssetMenu(fileName = "VfxSo", menuName = "So/VfxSo", order = 0)]
    public class VfxEffectsScriptableObject : UnityEngine.ScriptableObject
    {
        public List<VfxEffect> fxEffects = new List<VfxEffect>()
        {
            new VfxEffect()
            {
                fxType = VfxTypes.ButtonClickVfx
            }
        };
        
        public AudioClip GetVfxPrefab(VfxTypes fxType)
        {
            return (from fxEffect in fxEffects where fxEffect.fxType == fxType select fxEffect.fxClip).FirstOrDefault();
        }
    }
    
    [Serializable]
    public class VfxEffect
    {
        public VfxTypes fxType;
        public AudioClip fxClip;
    }
}