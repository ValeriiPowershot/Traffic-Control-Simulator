using System.Collections;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;

namespace BaseCode.Logic
{
    public class FxManager : ManagerBase
    {
        public FxEffectsScriptableObject fxEffectsSo;

        public GameObject PlayFx(FxTypes fxType, Transform parent)
        {
            var fxPrefab = fxEffectsSo.GetFxPrefab(fxType);

            if (fxPrefab == null)
            {
                Debug.Log("Effect Prefab not found.");
                return null;
            }

            var createdFx = Instantiate(fxPrefab, parent);

            var particleEffectComponent = createdFx.GetComponent<ParticleSystem>();
            StartCoroutine(DestroyAfterParticleEffect(particleEffectComponent, createdFx));
            return createdFx;
        }

        private IEnumerator DestroyAfterParticleEffect(ParticleSystem particleEffect, GameObject createdFx)
        {
            yield return new WaitWhile(() => particleEffect.IsAlive(true));
            Destroy(createdFx);
        }
    }
}