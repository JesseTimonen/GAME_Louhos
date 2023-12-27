using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents Instance { get; private set; }

    [field: Header("Overworld Ambience SFX")]
    [field: SerializeField]
    public EventReference OverworldAmbience { get; private set; }

    [field: Header("Cave Ambience SFX")]
    [field: SerializeField]
    public EventReference CaveAmbience { get; private set; }

    [field: Header("Music")]
    [field: SerializeField]
    public EventReference Music { get; private set; }

    [field: Header("Footsteps SFX")]
    [field: SerializeField]
    public EventReference Footsteps { get; private set; }

    [field: Header("Digging SFX")]
    [field: SerializeField]
    public EventReference Digging { get; private set; }

    [field: Header("Climbing SFX")]
    [field: SerializeField]
    public EventReference Climbing { get; private set; }

    [field: Header("Page turn SFX")]
    [field: SerializeField]
    public EventReference PageTurn { get; private set; }
    
    [field: Header("Jump SFX")]
    [field: SerializeField]
    public EventReference Jump { get; private set; }
    
    [field: Header("Land SFX")]
    [field: SerializeField]
    public EventReference Land { get; private set; }
    
    [field: Header("Item Place SFX")]
    [field: SerializeField]
    public EventReference ItemPlace { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found another instance of FMODEvents. Destroying this one.");
        }

        Instance = this;
    }
}