using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    struct Clamp<T>
    {
        public Clamp(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public readonly T Min;
        public readonly T Max;
    }

    private readonly Clamp<float> playerDepthParameterClamp = new(-256f, 32f);

    [SerializeField] private GameObject player;

    private List<EventInstance> audioEvents;
    private EventInstance overworldAmbienceEventInstance;
    private EventInstance caveAmbienceEventInstance;
    private EventInstance musicEventInstance;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found another instance of AudioManager. Destroying this one.");
        }

        Instance = this;
        audioEvents = new List<EventInstance>();
    }


    private void Start()
    {
        InitializeAmbience(FMODEvents.Instance.OverworldAmbience, FMODEvents.Instance.CaveAmbience);
        InitializeMusic(FMODEvents.Instance.Music);
        InvokeRepeating("SetPlayerDepthParameter", 0.5f, 0.5f);
    }


    private void InitializeAmbience(EventReference overworldAmbienceEventReference, EventReference caveAmbienceEventReference)
    {
        overworldAmbienceEventInstance = CreateEventInstance(overworldAmbienceEventReference);
        overworldAmbienceEventInstance.start();

        caveAmbienceEventInstance = CreateEventInstance(caveAmbienceEventReference);
        caveAmbienceEventInstance.start();
    }


    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }


    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        var eventInstance = RuntimeManager.CreateInstance(eventReference);
        audioEvents.Add(eventInstance);
        return eventInstance;
    }


    private void SetPlayerDepthParameter()
    {
        var depth = Math.Clamp(
            player.transform.position.y,
            playerDepthParameterClamp.Min,
            playerDepthParameterClamp.Max
        );

        overworldAmbienceEventInstance.setParameterByName("PlayerDepth", depth);
        caveAmbienceEventInstance.setParameterByName("PlayerDepth", depth);
        musicEventInstance.setParameterByName("PlayerDepth", depth);
    }


    public void Dig(Vector3 pos, float dmg)
    {
        PlayOneShot(FMODEvents.Instance.Digging, pos);
    }


    public void Climb(Vector3 pos, float speed)
    {
        if (speed <= 0)
        {
            return;
        }

        PlayOneShot(FMODEvents.Instance.Climbing, pos);
    }


    public void Walk(Vector3 pos, float speed)
    {
        if (speed <= 0)
        {
            return;
        }

        PlayOneShot(FMODEvents.Instance.Footsteps, pos);
    }


    public void PageTurn()
    {
        PlayOneShot(FMODEvents.Instance.PageTurn, player.transform.position);
    }


    public void Jump(Vector3 pos, float force)
    {
        PlayOneShot(FMODEvents.Instance.Jump, pos);
    }


    public void Land(Vector3 pos, float force)
    {
        PlayOneShot(FMODEvents.Instance.Land, pos);
    }
  


    public void PlaceItem()
    {
        PlayOneShot(FMODEvents.Instance.ItemPlace, player.transform.position);
    }


    private void PlayOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }


    private void OnDestroy()
    {
        foreach (var audioEvent in audioEvents)
        {
            audioEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            audioEvent.release();
        }
    }
}
