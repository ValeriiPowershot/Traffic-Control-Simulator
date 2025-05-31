using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public static class AudioHelper
{
    // Play a 2D SFX (menu click, UI, etc)
    public static void Play2DSFX(string eventPath)
    {
        RuntimeManager.PlayOneShot(eventPath);
    }

    // Play a 3D SFX attached to a GameObject (e.g., frog jumps) very short, for lingering sounds use soundemitters prefabs. 
    public static void Play3DSFXAttached(string eventPath, GameObject target)
    =>
        RuntimeManager.PlayOneShotAttached(eventPath, target);
    

 public static void Play3DSFXAttached(EventReference eventReference, GameObject target)
    =>
        RuntimeManager.PlayOneShotAttached(eventReference, target);
    
    // Play a 3D SFX at a specific world position (e.g., splash at a river)
    public static void Play3DSFXAtPosition(string eventPath, Vector3 position)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventPath);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        instance.release(); // Important: releases memory after playback
    }

public static void Play3DSFXAtPosition(EventReference eventReference, Vector3 position)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        instance.release(); // Important: releases memory after playback
    }



    public static void Play3DSFXAttachedWithParameters(string fmodEvent, Vector3 position, params (string name, object value)[] parameters)
    {
        EventInstance instance = RuntimeManager.CreateInstance(fmodEvent);

        foreach (var (name, value) in parameters)
        {
            switch (value)
            {
                case float floatValue:
                    instance.setParameterByName(name, floatValue);
                    break;
                case int intValue:
                    instance.setParameterByName(name, intValue);
                    break;
                case string labelValue:
                    instance.setParameterByNameWithLabel(name, labelValue);
                    break;
                case Enum enumValue:
                    instance.setParameterByName(name, Convert.ToSingle(enumValue));
                    break;
                // Add more cases as needed
            }
        }

        instance.set3DAttributes(position.To3DAttributes());
        instance.start();
        instance.release();
    }


    public static class EventPath
    {
        public const string MX_GameMusic = "event:/Music/MX_GameMusic";
        public const string MX_MainTheme = "event:/Music/MX_MainTheme";
        public const string SFX_Collision = "event:/SFX/SFX_Collision";
        public const string SFX_TrafficLight = "event:/SFX/SFX_TrafficLight"; // <-- Add this line
    }

    public static class ParameterName
    {
        public const string Intensity = "Intensity";
    }

}