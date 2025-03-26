using System;
using System.Collections.Generic;
using BaseCode.Extensions.UI;
using BaseCode.Logic.PopUps.Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace BaseCode.Logic
{
    public class PopUpManager : ManagerBase<PopUpManager>
    {
        public override GameManager GameManager
        {
            get
            {
                var baseGameManager = base.GameManager;
                baseGameManager.popUpManager = this; 
                return baseGameManager; 
            }
        }
        
        public PopUpBase initialPopUp; 
        public List<PopUpBase> popUpBases = new List<PopUpBase>();
        private PopUpBase _lastActivePopUpBase;

        public override void Start()
        {
            base.Start();
            ShowPopUpFromObject(initialPopUp);
        }
        
        public T ShowPopUp<T>() where T : PopUpBase
        {
            HidePopUp(_lastActivePopUpBase);
            
            _lastActivePopUpBase = GetPopUp<T>();
            ShowPopUpFromBase(_lastActivePopUpBase);

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
        
        public void ShowPopUpFromBase(PopUpBase popUpBase)
        {
            popUpBase?.gameObject.SetActive(true);
            popUpBase?.OnStartDoTween(); 
        }

        private void ShowPopUpFromObject(PopUpBase popUpBase)
        {
            var popUpType = popUpBase.GetType();
            typeof(PopUpManager)
                .GetMethod(nameof(ShowPopUp))
                ?.MakeGenericMethod(popUpType)
                .Invoke(this, null);
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