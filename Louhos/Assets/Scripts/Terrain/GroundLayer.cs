using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public struct GroundLayer
{
    public string Name;
    public int Height;
    public List<Valuable> Valuables;
    public Grounds Ground;
}
