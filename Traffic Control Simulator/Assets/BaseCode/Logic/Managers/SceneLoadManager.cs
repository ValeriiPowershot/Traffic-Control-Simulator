using System.Collections;
using BaseCode.Logic.ScriptableObject;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace BaseCode.Logic.Managers
{
    public class SceneLoadManager : SingletonManagerBase<SceneLoadManager>
    {
        public void LoadSceneNormal(SceneID id, UnityAction beforeLoad = null,UnityAction afterLoad = null)
        {
            var sceneName = SceneSo.GetScenesFromId(id);
            StartCoroutine(LoadSceneNormally(sceneName.sceneAsset.name, beforeLoad, afterLoad));
        }
        private IEnumerator LoadSceneNormally(string sceneName, UnityAction beforeLoad = null,UnityAction afterLoad = null)
        {
            beforeLoad?.Invoke();
            yield return SceneManager.LoadSceneAsync(sceneName);
            afterLoad?.Invoke();
        }
        private SceneSo SceneSo => GameManager.saveManager.sceneSo;
    }
}