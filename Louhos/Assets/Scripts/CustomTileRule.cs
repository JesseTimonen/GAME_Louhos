using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new_tile", menuName = "2D/Tiles/Custom Rule Tile")]
public class CustomTileRule : RuleTile
{
    // Overrides the default neighbor matching behavior to smoothly blend all tiles together
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        // If there is no tile, match only if the neighbor's index is default empty state
        if (tile == null)
        {
            return (neighbor == 2);
        }

        // I have no idea why/how this works, but it just does..
        switch (neighbor)
        {
            case TilingRuleOutput.Neighbor.This: return true;
            case TilingRuleOutput.Neighbor.NotThis: return false;
        }

        // For other cases fallback to the default rule match
        return base.RuleMatch(neighbor, tile);
    }
}