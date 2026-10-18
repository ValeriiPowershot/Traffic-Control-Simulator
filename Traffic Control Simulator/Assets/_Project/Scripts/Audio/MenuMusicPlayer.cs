using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[DisallowMultipleComponent]
public class MenuMusicPlayer : MonoBehaviour
{
    [SerializeField] private EventReference _menuMusic;

    private EventInstance _menuMusicInstance;
    
    private void Start()
    {
        _menuMusicInstance = RuntimeManager.CreateInstance(_menuMusic);
        _menuMusicInstance.start();
    }
}
