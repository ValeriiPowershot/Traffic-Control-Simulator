using System.Collections.Generic;
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
        public static Tween SequenceOpenerSetActive(this List<Transform> target, float durationBetween = 0.1f, int amount = -1)
        {
            if (target == null) return null;
            if(amount == -1) amount = target.Count;
    
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < amount; i++)
            {
                var i1 = i;
                var tween = target[i].DoPopUp().OnStart(()=>{target[i1].gameObject.SetActive(true);});
                sequence.Append(tween);
                sequence.AppendInterval(durationBetween);
            }
    
            return sequence;
        }
    }
}