using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[Serializable]
public struct Valuable
{
    public Valuables Name;
    [Range(0,1)]
    public float Chance;
}

public enum Valuables
{
    Gold,
    Amethyst,
    Diamond,
    Torch,
    StaminaPotion
}

[Serializable]
public struct ValuablesTiles
{
    public Valuables Name;
    public TileBase Tile;
}