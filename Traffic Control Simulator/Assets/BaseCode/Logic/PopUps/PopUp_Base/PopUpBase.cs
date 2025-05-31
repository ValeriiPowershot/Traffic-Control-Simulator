using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace BaseCode.Logic.PopUps.PopUp_Base
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PopUpBase : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        private void OnValidate()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void OnStartShow()
        {
        }
        
        public virtual void OnStartHidden()
        {
        }

        public void UIShow(Action onFinished = null)
        {
            if (CanvasGroup != null)
                CanvasGroup.DOFade(1, 0.1f)
                    .OnComplete(() => onFinished?.Invoke());
        }

        public void UIHide(Action onFinished = null)
        {
            if (CanvasGroup != null)
                CanvasGroup.DOFade(0, 0.5f)
                    .OnComplete(() => onFinished?.Invoke());
        }
        
        public virtual void OnStartDoTween(Action onFinished = null)
        {
            UIShow(onFinished);
            transform.SetSiblingIndex(transform.parent.childCount - 1);
        }
    }
}