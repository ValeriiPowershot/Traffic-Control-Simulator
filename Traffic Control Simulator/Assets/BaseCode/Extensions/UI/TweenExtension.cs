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
    }
}