using DG.Tweening;
using UnityEngine;

namespace BaseCode.Extensions.UI
{
    public static class TransformExtension
    {
        public static Tween DoPopUp(this Transform target, float duration = 0.3f)
        {
            if (target == null) return null;
    
            target.localScale = Vector3.zero; 
    
            return target.DOScale(1f, duration)
                .SetEase(Ease.Linear); 
        }
        public static Tween ReverseDoPopUp(this Transform target, float duration = 0.3f)
        {
            if (target == null) return null;
    
            target.localScale = Vector3.one; 
    
            return target.DOScale(0f, duration)
                .SetEase(Ease.Linear); 
        }
    }
}