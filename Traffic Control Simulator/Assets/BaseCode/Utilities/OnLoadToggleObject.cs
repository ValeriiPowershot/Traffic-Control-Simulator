using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaseCode.Utilities
{
    // dont check this file - it will be reviewed!
    public class OnLoadToggleObject : MonoBehaviour
    {
        public List<GameObject> toggleGameObjects = new List<GameObject>();
        
        public bool isOnStartActive = false;

        private void Start()
        {
            ToggleObjects();
        }

        public void ToggleObjects()
        {
            isOnStartActive = !isOnStartActive;
            foreach (var toggleGameObject in toggleGameObjects)
            {
                toggleGameObject.SetActive(isOnStartActive);
            }
        }
        
    }
}