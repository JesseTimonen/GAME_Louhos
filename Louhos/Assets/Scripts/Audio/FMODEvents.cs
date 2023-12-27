using UnityEngine;
using FMODUnity;


public class FMODEvents : MonoBehaviour
{
    public static FMODEvents Instance { get; private set; }
    [field: SerializeField] public EventReference OverworldAmbience { get; private set; }
    [field: SerializeField] public EventReference CaveAmbience { get; private set; }
    [field: SerializeField] public EventReference Music { get; private set; }
    [field: SerializeField] public EventReference Footsteps { get; private set; }
    [field: SerializeField] public EventReference Digging { get; private set; }
    [field: SerializeField] public EventReference Climbing { get; private set; }
    [field: SerializeField] public EventReference PageTurn { get; private set; }
    [field: SerializeField] public EventReference Jump { get; private set; }
    [field: SerializeField] public EventReference Land { get; private set; }
    [field: SerializeField] public EventReference ItemPlace { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found another instance of FMODEvents. Destroying this one.");
        }

        Instance = this;
    }
}
