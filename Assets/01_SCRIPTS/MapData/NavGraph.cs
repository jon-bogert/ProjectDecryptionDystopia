using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NavNode
{
    public TileBase tile = null;
    //
    public NavNode north = null;
    public NavNode east = null;
    public NavNode south = null;
    public NavNode west = null;
    public bool visited = false;

    internal void RecursiveProbe(TileMap3D map)
    {
        Vector3Int coord = tile.gridCoord;
        ProbeDir(map, north, coord + Vector3Int.forward, TileRotation.North);
        ProbeDir(map, east, coord + Vector3Int.right, TileRotation.East);
        ProbeDir(map, south, coord + Vector3Int.back, TileRotation.South);
        ProbeDir(map, west, coord + Vector3Int.left, TileRotation.West);
    }

    void ProbeDir(TileMap3D map, NavNode node, Vector3Int coord, TileRotation rot)
    {
        if (node.tile.id == TileType.Block)
        {
            
        }
    }
}

public class NavGraph : MonoBehaviour
{
    SortedDictionary<Vector3Int, NavNode> nodes;

    public void ResetVisitFlags()
    {
        foreach (var node in nodes)
        {
            node.Value.visited = false;
        }
    }

    public void GenerateGraph(TileMap3D tileMap)
    {
        ResetVisitFlags();
        TileBase playerTile = tileMap.playerStart;
        Vector3Int startCoord = playerTile.gridCoord;
        startCoord.y -= 1;
        TileBase startTile = tileMap.TileAt(startCoord);
        if (startTile != null && startTile.id == TileType.Block)
        {
            NavNode startNode = nodes[startCoord] = new NavNode();
            startNode.tile = startTile;
            startNode.visited = true;
            startNode.RecursiveProbe(tileMap);
        }

    }
}