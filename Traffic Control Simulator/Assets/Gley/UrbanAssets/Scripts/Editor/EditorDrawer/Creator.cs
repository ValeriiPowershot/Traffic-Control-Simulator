using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class Creator : UnityEditor.Editor
    {
        private readonly bool showDebugMessages = false;

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
