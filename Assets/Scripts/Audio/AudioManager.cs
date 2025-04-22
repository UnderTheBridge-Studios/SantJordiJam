using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 0.35f;
    [Range(0, 1)]
    public float musicVolume = 1f;
    [Range(0, 1)]
    public float ambienceVolume = 1f;
    [Range(0, 1)]
    public float SFXVolume = 1f;
    [Range(0, 1)]

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    private EventInstance barulloEventInstance;
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        RuntimeManager.LoadBank("Master", true);
        RuntimeManager.LoadBank("Master.strings", true);
        RuntimeManager.LoadBank("Level", true);
        RuntimeManager.LoadBank("Ambience", true);
        RuntimeManager.LoadBank("SFX", true);
        RuntimeManager.LoadBank("OST paralela", true);


        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        ambienceEventInstance = CreateInstance(FMODEvents.instance.ambience);
        musicEventInstance = CreateInstance(FMODEvents.instance.music);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        ambienceBus.setVolume(ambienceVolume);
        sfxBus.setVolume(SFXVolume);
    }

    public void InitializeSound()
    {

        PlayOneShot(FMODEvents.instance.start, transform.position);
        InitializeAmbience();
    }

    public void PlayMusica()
    {
        InitializeMusic();
    }

    public void PlayBarullo()
    {
        barulloEventInstance = CreateInstance(FMODEvents.instance.barullo);
        barulloEventInstance.start();
    }

    public void StopBarullo()
    {
        barulloEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void InitializeAmbience()
    {
        ambienceEventInstance.start();
    }

    private void InitializeMusic()
    {
        musicEventInstance.start();
    }

    public void StopSounds()
    {
        //ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicParameter(string parameterName, float parameterValue)
    {
        musicEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicTime(string time)
    {
        float floatTime = 0;
        if(time == "Day")
        {
            floatTime = 0;
        }
        else
        {
            floatTime = 1;
        }
        musicEventInstance.setParameterByName("ChangeDayAndNight", floatTime);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach(StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}