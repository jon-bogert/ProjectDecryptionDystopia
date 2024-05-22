using System.Collections.Generic;
using UnityEngine;
using XephTools;

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

        if (tile.type == TileType.Slope)
        {
            RotatableTile rt = tile as RotatableTile;
            switch (rt.Rotation)
            {
                case TileRotation.North:
                    north = ProbeDirection(map, graph, north, coord + Vector3Int.forward, TileRotation.North);
                    south = ProbeDirection(map, graph, south, coord + Vector3Int.back + Vector3Int.down, TileRotation.South);
                    break;
                case TileRotation.East:
                    east = ProbeDirection(map, graph, east, coord + Vector3Int.right, TileRotation.East);
                    west = ProbeDirection(map, graph, west, coord + Vector3Int.left + Vector3Int.down, TileRotation.West);
                    break;
                case TileRotation.South:
                    south = ProbeDirection(map, graph, south, coord + Vector3Int.back, TileRotation.South);
                    north = ProbeDirection(map, graph, north, coord + Vector3Int.forward + Vector3Int.down, TileRotation.North);
                    break;
                default:
                    west = ProbeDirection(map, graph, west, coord + Vector3Int.left, TileRotation.West);
                    east = ProbeDirection(map, graph, east, coord + Vector3Int.right + Vector3Int.down, TileRotation.East);
                    break;
            }
            return;
        }

        north = ProbeDirection(map, graph, north, coord + Vector3Int.forward, TileRotation.North);
        east = ProbeDirection(map, graph, east, coord + Vector3Int.right, TileRotation.East);
        south = ProbeDirection(map, graph, south, coord + Vector3Int.back, TileRotation.South);
        west = ProbeDirection(map, graph, west, coord + Vector3Int.left, TileRotation.West);
    }

    NavNode ProbeDirection(TileMap3D map, NavGraph graph, NavNode node, Vector3Int coord, TileRotation rot)
    {
        TileBase checkTile = map.TileAt(coord);
        if (checkTile == null) // Not in Map
            return null;

        if (checkTile.type == TileType.Block)
        {
            //Check block above
            TileBase tileAbove = map.TileAt(coord + Vector3Int.up);
            if (tileAbove.type != TileType.Slope &&
                tileAbove.type != TileType.Space &&
                tileAbove.type != TileType.EnemyMelee &&
                tileAbove.type != TileType.EnemyRanged &&
                tileAbove.type != TileType.PlayerStart &&
                tileAbove.type != TileType.Key)
            {
                //Blocked path
                return null;
            }

            //Check that slope is in correct direction
            if (tileAbove.type == TileType.Slope)
            {
                //Check one more above
                TileBase anotherTileAbove = map.TileAt(coord + Vector3Int.up * 2);
                if (anotherTileAbove.type != TileType.Space &&
                    anotherTileAbove.type != TileType.EnemyMelee &&
                    anotherTileAbove.type != TileType.EnemyRanged &&
                    anotherTileAbove.type != TileType.PlayerStart &&
                    tileAbove.type != TileType.Key)
                    return null;

                RotatableTile rt = tileAbove as RotatableTile;
                if (rt.Rotation != rot)
                {
                    //Wrong Direction
                    return null;
                }
                //Check if Node already exists
                if (graph.nodes.ContainsKey(tileAbove.gridCoord))
                {
                    node = graph.nodes[tileAbove.gridCoord];
                    return node;
                }
                else
                {
                    node = new NavNode();
                    node.tile = tileAbove;
                    graph.nodes[node.coord] = node;
                    node.RecursiveProbe(map, graph);
                }
                return node;
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
            return node;
        }
        //Check space
        else if (checkTile.type == TileType.Slope)
        {
            if (checkTile.type != TileType.Slope)
            {
                //Blocked path
                return null;
            }

            //Check that slope is in correct direction
            if (checkTile.type == TileType.Slope)
            {
                RotatableTile rt = checkTile as RotatableTile;
                TileRotation desiredDir = (TileRotation)(((int)rot + 2) % 4);
                if (rt.Rotation != desiredDir)
                {
                    //Wrong Direction
                    return null;
                }
                //Check if Node already exists
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
        }
        return node;
    }
}

public class NavGraph : MonoBehaviour
{
    public Dictionary<Vector3Int, NavNode> nodes;

    public void ResetVisitFlags()
    {
        foreach (var node in nodes)
        {
            node.Value.visited = false;
        }
    }


    public void GenerateGraph(TileMap3D tileMap)
    {
        nodes = new Dictionary<Vector3Int, NavNode>();
        ResetVisitFlags();
        //TileBase playerTile = tileMap.playerStart;
        //Vector3Int startCoord = playerTile.gridCoord;
        //startCoord.y -= 1;
        tileMap.ForEach((TileBase tile) =>
        {
            TileBase tileAbove = tileMap.TileAt(tile.gridCoord + Vector3Int.up);
            if (tile.type != TileType.Block || tileAbove == null || tileAbove.type != TileType.Space)
                return;

            if (nodes.ContainsKey(tile.gridCoord))
                return;
            NavNode startNode = nodes[tile.gridCoord] = new NavNode();
            startNode.tile = tile;
            startNode.RecursiveProbe(tileMap, this);
        });

    }

#if UNITY_EDITOR
    const float OFFSET_AMT = 0.05f;
    private void DrawBranch(NavNode node, Vector3 startPos, Vector3 offsetDir)
    {
        Vector3 endPos = node.tile.gameObject.transform.position;
        endPos.y += .5f;
        Vector3 offset = offsetDir * OFFSET_AMT;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(startPos + offset, endPos + offset);
    }

    private void OnDrawGizmosSelected()
    {
        if (nodes == null)
            return;

        foreach (var nodePair in nodes)
        {
            NavNode node = nodePair.Value;
            Vector3 pos = node.tile.gameObject.transform.position;
            pos.y += .5f;

            Gizmos.color = new Color32(113, 96, 232, 255);
            //Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(pos, 0.1f);

            if (node.north != null)
                DrawBranch(node.north, pos, Vector3.right);
            if (node.west != null)
                DrawBranch(node.west, pos, Vector3.back);
            if (node.south != null)
                DrawBranch(node.south, pos, Vector3.left);
            if (node.east != null)
                DrawBranch(node.east, pos, Vector3.forward);
        }
    }
#endif // UNITY_EDITOR

    public List<NavNode> Dijkstra(NavNode start, NavNode end)
    {
        ResetVisitFlags();
        PriorityQueue<NavNode> openSet = new ();
        Dictionary<Vector3Int, bool> closedSet = new();
        Dictionary<Vector3Int, NavNode> cameFrom = new();
        Dictionary<Vector3Int, float> gScore = new();

        openSet.Enqueue(start, 0);
        gScore[start.tile.gridCoord] = 0;

        while (!openSet.IsEmpty)
        {
            NavNode current = openSet.Dequeue();

            if (current.tile.gridCoord == end.tile.gridCoord)
            {
                return ReconstructPath(cameFrom, current);
            }

            closedSet[current.tile.gridCoord] = true;

            foreach (NavNode neighbor in GetNeighbors(current))
            {
                if (closedSet.ContainsKey(neighbor.tile.gridCoord))
                {
                    continue;
                }

                float tentativeGScore = gScore[current.tile.gridCoord] + DistanceBetween(current, neighbor);

                if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor.tile.gridCoord])
                {
                    cameFrom[neighbor.tile.gridCoord] = current;
                    gScore[neighbor.tile.gridCoord] = tentativeGScore;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Enqueue(neighbor, gScore[neighbor.tile.gridCoord]);
                    }
                }
            }
        }

        return null; // Path not found
    }

    private float DistanceBetween(NavNode a, NavNode b)
    {
        return Vector3Int.Distance(a.tile.gridCoord, b.tile.gridCoord);
    }

    private List<NavNode> GetNeighbors(NavNode node)
    {
        var neighbors = new List<NavNode>();

        if (node.north != null) neighbors.Add(node.north);
        if (node.east != null) neighbors.Add(node.east);
        if (node.south != null) neighbors.Add(node.south);
        if (node.west != null) neighbors.Add(node.west);

        return neighbors;
    }

    private List<NavNode> ReconstructPath(Dictionary<Vector3Int, NavNode> cameFrom, NavNode current)
    {
        var path = new List<NavNode>();
        path.Add(current);

        while (cameFrom.ContainsKey(current.tile.gridCoord))
        {
            current = cameFrom[current.tile.gridCoord];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}