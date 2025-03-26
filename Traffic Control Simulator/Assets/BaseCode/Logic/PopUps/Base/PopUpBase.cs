using BaseCode.Extensions.UI;
using DG.Tweening;
using UnityEngine;

namespace BaseCode.Logic.PopUps.Base
{
    public class PopUpBase : MonoBehaviour
    {
        public virtual void OnStartShow()
        {
            Debug.Log("OnStartShow " + GetType());
            
        }
        
        public virtual void OnStartHidden()
        {
            Debug.Log("OnStartHidden " + GetType());
            
        }

        public virtual void OnStartDoTween()
        {
            transform.SetSiblingIndex(transform.parent.childCount - 1);
            transform.DoPopUp(0.5f).OnComplete(OnStartShow);
        }
    }
}