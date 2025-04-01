using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BaseCode.Extensions.UI
{
    public static class TweenExtension
    {
        public static void OnCompleteCloseActiveness(this Tween tweener)
        {
            tweener.onComplete = () => ((Transform)tweener.target).gameObject.SetActive(false);
        }
        public static void PressedEffect(this Transform hitVehicle, float onYScale)
        {
            Vector3 originalScale = hitVehicle.localScale;
            
            Vector3 pressedScale = new Vector3(originalScale.x, originalScale.y * 0.1f, originalScale.z);
            hitVehicle.transform.DOScaleY(pressedScale.y, 0.5f).SetEase(Ease.OutQuad);
        }
       
    }
}