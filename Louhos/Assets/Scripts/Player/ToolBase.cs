using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Tool/Create new tool")]

public class ToolBase : ScriptableObject
{
    [field: SerializeField]
    public string Identifier { get; set; }

    [field: SerializeField]
    public float Damage { get; private set; }

    [field: SerializeField]
    public int Tier { get; private set; }

    [field: SerializeField]
    public float Range { get; private set; }

    [field: SerializeField]
    public float EnergyConsumption { get; private set; }
}
