using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour
{
    public static RandomSound Singleton;
    public AudioSource source;
    public AudioClip[] audioClips;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSourceClip(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}