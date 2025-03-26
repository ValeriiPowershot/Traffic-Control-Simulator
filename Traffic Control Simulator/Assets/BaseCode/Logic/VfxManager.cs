using System;
using System.Collections;
using BaseCode.Extensions;
using BaseCode.Logic.ScriptableObject;
using UnityEngine;
using UnityEngine.Events;

namespace BaseCode.Logic
{
    public class VfxManager : SingletonManagerBase<VfxManager>
    {
        public VfxEffectsScriptableObject fxEffectsSo;
        public AudioSource mainGameMusic;
        
        // game sound effect
        public AudioSource PlayVfx(VfxTypes fxType)
        {
            var fxPrefab = fxEffectsSo.GetVfxPrefab(fxType);
            if (fxPrefab == null)
            {
                Debug.LogError("Effect Prefab not found.");
                return null;
            }
            var vfxInstance = CreateVfxObject(fxPrefab);
            var audioSource = vfxInstance.GetComponent<AudioSource>();
            
            StartCoroutine(DestroyAfterVfxEffect(audioSource, vfxInstance.gameObject, () => GameSettings.soundVolume));
            return audioSource;
        }
        private IEnumerator DestroyAfterVfxEffect(AudioSource audioSource, GameObject createdVfx, Func<float> volumeChangeHandler)
        {
            audioSource.Play();
            
            while (audioSource.isPlaying)
            {
                audioSource.volume = volumeChangeHandler();
                yield return null; 
            }
            Destroy(createdVfx);
        }
        private AudioSource CreateVfxObject(AudioClip clip)
        {
            var vfxObject = new GameObject("VFX_AudioSource");
            var audioSource = vfxObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            return audioSource;
        }
        
        // game music
        public void PlayGameMusic(VfxTypes vfxTypes, float lastMusicTime = 0)
        {
            var newMusicClip = fxEffectsSo.GetVfxPrefab(vfxTypes);
            StartCoroutine(SetNewSoundWithFade(newMusicClip, lastMusicTime));
        }

        private IEnumerator SetNewSoundWithFade(AudioClip newMusicClip, float lastMusicTime = 0)
        {
            yield return FadeOutMusic(); 
            
            mainGameMusic.clip = newMusicClip;
            mainGameMusic.Play();
            mainGameMusic.time = lastMusicTime;
            
            yield return FadeInMusic(GameSettings.musicVolume, 1f); 
        }

        public IEnumerator FadeOutMusic(IteratorRefExtension<float> lastTime = null, float duration = 1)
        {
            float targetVolume = 0;
            float startVolume = mainGameMusic.volume;
            float time = 0f;

            while (time < duration)
            {
                mainGameMusic.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            mainGameMusic.volume = targetVolume;
            if (lastTime != null) lastTime.Value = mainGameMusic.time;
            mainGameMusic.Stop();
        }

        private IEnumerator FadeInMusic(float targetVolume, float duration)
        {
            mainGameMusic.volume = 0f; 
            float time = 0f;

            while (time < duration)
            {
                mainGameMusic.volume = Mathf.Lerp(0f, targetVolume, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            mainGameMusic.volume = targetVolume; 
        }
        
        public GameSettings GameSettings => GameManager.saveManager.configSo.gameSettings;
    
    }
}