using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class Data : UnityEditor.Editor
    {
        private readonly bool showDebugMessages = false;

        protected abstract void LoadAllData();


        internal delegate void Modified();
        internal event Modified onModified;
        internal void TriggerModifiedEvent()
        {
            if (showDebugMessages)
            {
                Debug.Log("Modified " + this);
            }
            LoadAllData();
            if (onModified != null)
            {
                onModified();
            }
        }


        protected void Initialize()
        {
            if (showDebugMessages)
            {
                Debug.Log("Initialize " + this);
            }
            LoadAllData();
        }


        private void OnEnable()
        {
            if (showDebugMessages)
            {
                Debug.Log("OnEnable " + this);
            }
        }


        protected virtual void OnDestroy()
        {
            if (showDebugMessages)
            {
                Debug.Log("OnDestroy " + this);
            }
        }
    }
}