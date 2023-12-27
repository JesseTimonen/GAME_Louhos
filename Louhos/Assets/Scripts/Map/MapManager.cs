using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class MapManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Vector3Int mapOffset = new(-12, -2, 0);
    [SerializeField] private int Width = 24;
    [SerializeField]
    [Tooltip("How many tiles in y axis to blend layerlines per direction")]
    private int layerOverlap;
    [SerializeField] private int wallThickness;

    [Header("Layers")]
    [SerializeField] private List<GroundLayer> Layers;
    [SerializeField] private List<ValuablesTiles> valuableTiles;
    [SerializeField] private List<GroundTiles> groundTiles;
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap valuablesTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase wallTile;

    private int Height => Layers.Sum(layer => layer.Height);
    private int fullWidth => Width + wallThickness * 2;
    private int[,] groundMap;
    private int[,] valuablesMap;
    private Random rnd;
    public UnityAction OnEndDig;

    [Header("Caves")]
    [SerializeField] private int numberOfCaves = 5;
    [SerializeField] private int maxCaveLength = 20;
    [SerializeField] [Range(0, 1f)] private float branchingChance = 0.1f;
    [SerializeField] private int branchingLimit = 2;
    [SerializeField] [Range(0, 1f)] private float chestSpawnChance = 0.01f;
    [SerializeField] private GameObject chestPrefab;


    private void Start()
    {
        rnd = new Random((int)DateTime.Now.Ticks);
        GenerateGroundArray();
        GenerateValuablesArray();
        GenerateCaves();
        RenderMaps();
    }


    void GenerateGroundArray()
    {
        groundMap = new int[fullWidth, Height + wallThickness];
        for (var layer = 0; layer < Layers.Count; layer++)
        {
            AddLayerToMap(ref groundMap, GenerateGroundLayer(layer), layer);
        }
        
    }


    void GenerateValuablesArray()
    {
        valuablesMap = new int[Width, Height];
        for (var layer = 0; layer < Layers.Count; layer++)
        {
            AddLayerToMap(ref valuablesMap, GenerateValuableLayer(layer), layer);
        }
    }


    private int[,] GenerateValuableLayer(int layer)
    {
        var height = Layers[layer].Height;
        var layerMap = new int[Width, height];
        int x = 0;
        int y = 0;
        foreach (var valuable in Layers[layer].Valuables)
        {
            (int name, int amount) val = ((int)valuable.Name, (int)math.floor(valuable.Chance * (Width * height)));
            if (val.amount <= 0)
                continue;
            for (;x < Width && val.amount > 0; x++)
            {
                for (; y < height && val.amount > 0; y++, val.amount--)
                {
                    layerMap[x, y] = val.name + 1;
                }
                y = 0;
            }
        }
        for (;x < Width; x++)
        {
            for (; y < height; y++)
            {
                layerMap[x, y] = 0;
            }
            y = 0;
        }
        

        return ShuffleArray(rnd, layerMap);
    }


    int[,] GenerateGroundLayer(int layer)
    {
        var layerInfo = Layers[layer]; 
        var height = layerInfo.Height;
        var layerMap = new int[Width, height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (height - y <= layerOverlap && layer < Layers.Count - 1) // Blending down
                {
                    layerMap[x, y] = rnd.NextDouble() < (double)(layerOverlap + height - y) / (layerOverlap * 2)
                        ? (int)layerInfo.Ground + 1
                        : (int)Layers[layer + 1].Ground + 1;
                }
                else if (y < layerOverlap && layer != 0) // Blending up
                {
                    layerMap[x, y] = rnd.NextDouble() < (double)(layerOverlap + y) / (layerOverlap * 2) ? (int)layerInfo.Ground + 1 : (int)Layers[layer -1].Ground + 1;
                }
                else // "normal"
                    layerMap[x, y] = (int)layerInfo.Ground + 1;
            }
        }

        
        return layerMap;
    }


    private void AddLayerToMap(ref int[,] map, int[,] layerMap, int layer)
    {
        var yOffset = Layers.Take(layer).Sum(l => l.Height);
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Layers[layer].Height; y++)
            {
                map[x, y + yOffset] = layerMap[x, y];
            }
        }
    }


    private void RenderMaps()
    {
        groundTilemap.ClearAllTiles();
        valuablesTilemap.ClearAllTiles();
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var groundTile = groundMap[x, y];
                if (groundTile != 0)
                {
                    groundTilemap.SetTile(new Vector3Int(x, -y, 0) + mapOffset, groundTiles.FirstOrDefault(ground => (int)ground.Name == (groundTile -1)).Tile);
                }

                var valTile = valuablesMap[x, y];
                if (valTile != 0)
                {
                    valuablesTilemap.SetTile(new Vector3Int(x, -y, 0) + mapOffset, valuableTiles.FirstOrDefault(valuable => (int)valuable.Name == (valTile -1)).Tile);
                }
            }
        }
        RenderWalls();
    }


    private void RenderWalls()
    {
        wallTilemap.ClearAllTiles();
        int extraHeight = 100;

        for (var x = 0; x < fullWidth; x++)
        {
            for (var y = -extraHeight; y < Height + wallThickness + extraHeight; y++)
            {
                if (x < wallThickness || x >= fullWidth - wallThickness)
                    wallTilemap.SetTile(new Vector3Int(x - wallThickness, -y, 0) + mapOffset, wallTile);
            }
        }
    }



    private static T[,] ShuffleArray<T>(Random random, T[,] array)
    {
        int lengthRow = array.GetLength(1);

        for (int i = array.Length - 1; i > 0; i--)
        {
            var i0 = i / lengthRow;
            var i1 = i % lengthRow;

            var j = random.Next(i + 1);
            int j0 = j / lengthRow;
            int j1 = j % lengthRow;

            (array[i0, i1], array[j0, j1]) = (array[j0, j1], array[i0, i1]);
        }

        return array;
    }


    public void Dig(Vector3 pos, float dmg)
    {
        var tileMapPos = groundTilemap.WorldToCell(pos);
        var mapPos = TileMapToMap(tileMapPos) * new Vector3Int(1, -1, 1);
        if (CellInsideMap(mapPos))
        {
            var groundTile = groundMap[mapPos.x, mapPos.y];
            var valuablesTile = valuablesMap[mapPos.x, mapPos.y];
            if (dmg >= groundTiles[groundTile-1].MinDamage)
            {
                // TODO Implement tile health here.
                if (groundTile != 0)
                {
                    GameManager.Instance.AddGround(groundTile - 1, 1);
                    groundTilemap.SetTile(new Vector3Int(tileMapPos.x, tileMapPos.y, 0), null);
                    groundMap[mapPos.x, mapPos.y] = 0;
                }
                if (valuablesTile != 0)
                {
                    GameManager.Instance.AddValuable(valuablesTile - 1, 1);
                    valuablesTilemap.SetTile(new Vector3Int(tileMapPos.x, tileMapPos.y, 0), null);
                    valuablesMap[mapPos.x, mapPos.y] = 0;
                }
            }
        }
        OnEndDig?.Invoke();
    }


    private bool CellInsideMap(Vector3Int pos)
    {
        return pos is { x: >= 0, y: >= 0 } && pos.x < Width && pos.y < Height;  
    }


    private Vector3Int TileMapToMap(Vector3Int pos)
    {
        return pos - mapOffset;
    }


    private void GenerateCaves()
    {
        for (int i = 0; i < numberOfCaves; i++)
        {
            Vector2Int caveStart = new Vector2Int(rnd.Next(Width), rnd.Next(Height));
            CreateCaveBranch(caveStart, maxCaveLength);
        }
    }


    private void CreateCaveBranch(Vector2Int currentPoint, int maxLength, int currentDepth = 0)
    {
        if (currentDepth > branchingLimit) return;

        // Make branches shorter than main cave
        if (currentDepth != 0)
        {
            maxLength = (int)Mathf.Floor(maxLength / 2f);
        }

        // Get cave main direction
        Vector2Int caveDirection = RandomCaveDirection();

        for (int i = 0; i < maxLength; i++)
        {
            // Set cave to go into main direction with additional randomness
            currentPoint += caveDirection + RandomCaveDirection() + RandomCaveDirection();

            // Cancel if we have reaches edge of the map
            if (!IsInsideMapBounds(currentPoint)) break;

            CarveCaveAtPoint(currentPoint);

            // Create a new branch of the cave
            if (rnd.NextDouble() < branchingChance)
            {
                CreateCaveBranch(currentPoint, maxLength, currentDepth + 1);
            }

            // Chance to spawn a GameObject at this point
            if (rnd.NextDouble() < chestSpawnChance)
            {
                Instantiate(chestPrefab, new Vector3(currentPoint.x + mapOffset.x + 0.45f, -currentPoint.y - 1.25f, 0), Quaternion.identity);
            }
        }
    }


    private void CarveCaveAtPoint(Vector2Int point)
    {
        // Always carve middle and edges
        groundMap[point.x, point.y] = 0;
        groundMap[point.x + 1, point.y] = 0;
        groundMap[point.x - 1, point.y] = 0;
        groundMap[point.x, point.y + 1] = 0;
        groundMap[point.x, point.y - 1] = 0;
        valuablesMap[point.x, point.y] = 0;
        valuablesMap[point.x + 1, point.y] = 0;
        valuablesMap[point.x - 1, point.y] = 0;
        valuablesMap[point.x, point.y + 1] = 0;
        valuablesMap[point.x, point.y - 1] = 0;

        // 50% chance to also carve diagonal points for more natural cave edges
        if (rnd.NextDouble() < 0.5)
        {
            groundMap[point.x + 1, point.y + 1] = 0;
            groundMap[point.x + 1, point.y - 1] = 0;
            groundMap[point.x - 1, point.y + 1] = 0;
            groundMap[point.x - 1, point.y - 1] = 0;
            valuablesMap[point.x + 1, point.y + 1] = 0;
            valuablesMap[point.x + 1, point.y - 1] = 0;
            valuablesMap[point.x - 1, point.y + 1] = 0;
            valuablesMap[point.x - 1, point.y - 1] = 0;
        }
    }


    private Vector2Int RandomCaveDirection()
    {
        switch (rnd.Next(6))
        {
            case 0: return new Vector2Int(-1, 0);
            case 1: return new Vector2Int(1, 0);
            case 2: return new Vector2Int(1, 1);
            case 3: return new Vector2Int(1, -1);
            case 4: return new Vector2Int(-1, 1);
            case 5: return new Vector2Int(-1, -1);
            default: return Vector2Int.zero;
        }
    }


    private bool IsInsideMapBounds(Vector2Int point)
    {
        return point.x > 0 && point.x + 1 < Width && point.y > 5 && point.y + 10 < Height;
    }
}
