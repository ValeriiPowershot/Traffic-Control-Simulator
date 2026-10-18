using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SimpleMusicStarter : MonoBehaviour
{
    public EventReference musicTrack;
    private EventInstance instance;

    void Start()
    {
        // Создаем и сразу запускаем
        instance = RuntimeManager.CreateInstance(musicTrack);
        instance.start();

        // Сразу освобождаем (чтобы инстанс удалился сам после остановки)
        instance.release();
    }
}
