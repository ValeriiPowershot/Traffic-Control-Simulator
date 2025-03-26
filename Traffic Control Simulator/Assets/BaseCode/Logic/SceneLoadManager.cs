using System.Collections;
using System.Collections.Generic;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BaseCode.Logic
{
    public class SceneLoadManager : ManagerBase
    {
        private Scene? _currentScene = null;
        public void LoadScene(SceneID id, int index = 0, UnityAction beforeLoad = null,UnityAction afterLoad = null)
        {
            var sceneName = SceneSo.GetScenePathFromId(id,index);
            StartCoroutine(LoadSceneAdditively(sceneName.sceneAsset.name, beforeLoad, afterLoad));
        }

        public void UnLoadScene()
        {
            StartCoroutine(UnLoadLoadedScene());
        }

        private IEnumerator UnLoadLoadedScene()
        {
            if(!_currentScene.HasValue) yield break;
            yield return SceneManager.UnloadSceneAsync(_currentScene.Value);
        }

        private IEnumerator LoadSceneAdditively(string sceneName, UnityAction beforeLoad = null,UnityAction afterLoad = null)
        {
            if (_currentScene.HasValue)
            {
                yield return UnLoadLoadedScene();
            }

            beforeLoad?.Invoke();
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            afterLoad?.Invoke();
            
            _currentScene = SceneManager.GetSceneByName(sceneName);
        }
        
        
        private SceneSo SceneSo => gameManager.saveManager.sceneSo;
    }
}