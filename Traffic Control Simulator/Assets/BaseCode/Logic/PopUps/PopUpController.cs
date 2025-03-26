using System;
using System.Collections.Generic;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using DG.Tweening;
using UnityEngine;

namespace BaseCode.Logic.PopUps
{
    [Serializable]
    public class PopUpController
    {
        public List<PopUpBase> popUpBases = new List<PopUpBase>();
        private PopUpBase _lastActivePopUpBase;
        
        public T ShowPopUp<T>() where T : PopUpBase
        {
            HidePopUp(_lastActivePopUpBase);
            
            _lastActivePopUpBase = GetPopUp<T>();
            ShowPopUp(_lastActivePopUpBase);

            return (T)_lastActivePopUpBase;
        }
    
        public void HidePopUp<T>() where T : PopUpBase
        {
            var currentPopUp = GetPopUp<T>();
            HidePopUp(currentPopUp);
        }
        
        public void HidePopUp(PopUpBase popUpBase)
        {
            if (popUpBase == null)
                return;
            
            popUpBase.transform.ReverseDoPopUp(0.5f).OnCompleteCloseActiveness();
            popUpBase.OnStartHidden();
        }
        
        public void ShowPopUp(PopUpBase popUpBase)
        {
            popUpBase.gameObject.SetActive(true);
            popUpBase.OnStartDoTween(); 
        }
        
        public T GetPopUp<T>() where T : PopUpBase
        {
            foreach (var popUpBase in popUpBases)
            {
                if (popUpBase.GetType() == typeof(T))
                {
                    return (T)popUpBase;
                }
            }
            return null;
        }


    }
}