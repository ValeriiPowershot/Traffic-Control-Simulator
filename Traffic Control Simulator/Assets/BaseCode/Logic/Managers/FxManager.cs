using System.Collections;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Utilities;
using UnityEngine;

namespace BaseCode.Logic.Managers
{
    public class FxManager : SingletonManagerBase<FxManager>
    {
        public FxEffectsScriptableObject fxEffectsSo;

        public void PlayFx(FxTypes fxType, Transform parent, Vector3 localSize = default)
        {
            GameObject fxPrefab = fxEffectsSo.GetFxPrefab(fxType);

            if (fxPrefab == null)
            {
                Debug.Log("Effect Prefab not found.");
                return;
            }
            GameObject createdFx = Instantiate(fxPrefab);
            createdFx.transform.position = parent != null ? parent.position : Vector3.zero;
            createdFx.transform.localScale = localSize;

            if (parent != null)
            {
                createdFx.transform.SetParent(parent, true);
            }

            createdFx.AddComponent<LookAtCamera>();
            ParticleSystem particleEffectComponent = createdFx.GetComponent<ParticleSystem>();
            StartCoroutine(DestroyAfterParticleEffect(particleEffectComponent, createdFx));
        }

        private IEnumerator DestroyAfterParticleEffect(ParticleSystem particleEffect, GameObject createdFx)
        {
            yield return new WaitWhile(() => particleEffect.IsAlive(true));
            Destroy(createdFx);
        }
    }
}