using System;
using UnityEngine.Tilemaps;


public enum Grounds
{
    Dirt,
    Stone,
    Bedrock,
    DragonStone
}


[Serializable]
public struct GroundTiles
{
    public Grounds Name;
    public TileBase Tile;
    public float MinDamage;
    public float MaxHealth;
}
