using System.Collections;
using BaseCode.Logic.ScriptableObject;
using BaseCode.Logic.Utilities;
using UnityEngine;

namespace BaseCode.Logic
{
    public class FxManager : ManagerBase
    {
        public FxEffectsScriptableObject fxEffectsSo;

        public void PlayFx(FxTypes fxType, Transform parent)
        {
            GameObject fxPrefab = fxEffectsSo.GetFxPrefab(fxType);

            if (fxPrefab == null)
            {
                Debug.Log("Effect Prefab not found.");
                return;
            }

            GameObject createdFx = Instantiate(fxPrefab, parent);

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