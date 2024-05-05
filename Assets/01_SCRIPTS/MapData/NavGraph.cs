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

    public Vector3Int coord { get { return tile.gridCoord; } }

    internal void RecursiveProbe(TileMap3D map, NavGraph graph)
    {
        Vector3Int coord = tile.gridCoord;
        ProbeDir(map, graph, north, coord + Vector3Int.forward, TileRotation.North);
        ProbeDir(map, graph, east, coord + Vector3Int.right, TileRotation.East);
        ProbeDir(map, graph, south, coord + Vector3Int.back, TileRotation.South);
        ProbeDir(map, graph, west, coord + Vector3Int.left, TileRotation.West);
    }

    void ProbeDir(TileMap3D map, NavGraph graph, NavNode node, Vector3Int coord, TileRotation rot)
    {
        TileBase checkTile = map.TileAt(coord);
        if (checkTile.type == TileType.Block)
        {
            //Check block above
            TileBase tileAbove = map.TileAt(coord + Vector3Int.up);
            if (tileAbove.type != TileType.Slope && tileAbove.type != TileType.Space)
            {
                //Blocked path
                return;
            }

            //Check that slope is in correct direction
            if (tileAbove.type == TileType.Slope)
            {
                //Check one more above
                if (map.TileAt(coord + Vector3Int.up * 2).type != TileType.Space)
                    return;

                RotatableTile rt = tileAbove as RotatableTile;
                if (rt.Rotation != rot)
                {
                    //Wrong Direction
                    return;
                }
                //Check if Node already exists
                if (graph.nodes.ContainsKey(tileAbove.gridCoord))
                {
                    node = graph.nodes[tileAbove.gridCoord];
                }
                else
                {
                    node = new NavNode();
                    node.tile = tileAbove;
                    graph.nodes[node.coord] = node;
                    node.RecursiveProbe(map, graph);
                }
                return;
            }

            //Space above Normal spot
            if (graph.nodes.ContainsKey(checkTile.gridCoord))
            {
                node = graph.nodes[checkTile.gridCoord];
            }
            else
            {
                node = new NavNode();
                node.tile = checkTile;
                graph.nodes[node.coord] = node;
                node.RecursiveProbe(map, graph);
            }
        }
        //Check space
        else if (checkTile.type == TileType.Space)
        {
            //Check block below
            TileBase tileBelow = map.TileAt(coord + Vector3Int.down);
            if (tileBelow.type != TileType.Slope)
            {
                //Blocked path
                return;
            }

            //Check that slope is in correct direction
            if (tileBelow.type == TileType.Slope)
            {

                RotatableTile rt = tileBelow as RotatableTile;
                if (rt.Rotation != (rot + 2 % 4))
                {
                    //Wrong Direction
                    return;
                }
                //Check if Node already exists
                if (graph.nodes.ContainsKey(tileBelow.gridCoord))
                {
                    node = graph.nodes[tileBelow.gridCoord];
                }
                else
                {
                    node = new NavNode();
                    node.tile = tileBelow;
                    graph.nodes[node.coord] = node;
                    node.RecursiveProbe(map, graph);
                }
                return;
            }
        }
    }
}

public class NavGraph : MonoBehaviour
{
    public SortedDictionary<Vector3Int, NavNode> nodes;

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
        if (startTile != null && startTile.type == TileType.Block)
        {
            NavNode startNode = nodes[startCoord] = new NavNode();
            startNode.tile = startTile;
            startNode.RecursiveProbe(tileMap, this);
        }

    }
}