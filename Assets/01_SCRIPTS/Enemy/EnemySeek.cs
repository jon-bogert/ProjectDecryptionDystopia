using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySeek : MonoBehaviour
{
    [SerializeField] float _arrivalDistance = 0.5f;
    [SerializeField] bool _disableNav = false;

    [Space]
    [Header("Debug")]
    [SerializeField] bool _showDebug = false;

    NavGraph _navGraph;
    LevelManager _level;
    List<NavNode> _path = null;

    private void Start()
    {
        _navGraph = FindObjectOfType<NavGraph>();
        if (_navGraph == null && !_disableNav)
            Debug.LogError(name + " could not find NavGraph in current scene");

        _level = FindObjectOfType<LevelManager>();
        if (_level == null)
            Debug.LogError(name + " could not find LevelManager in current scene");
    }

    private void OnDrawGizmos()
    {
        if (!_showDebug)
            return;

        Vector3 arrivalMax = transform.position + transform.forward * _arrivalDistance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, arrivalMax);

        //Runtime only
        if (_path != null && _level != null)
        {
            for (int i = 1; i < _path.Count; i++)
            {
                Gizmos.color = new Color(255, 165, 0);
                Vector3 start = _level.transform.TransformPoint(_path[i - 1].coord);
                Vector3 end = _level.transform.TransformPoint(_path[i].coord);
                start.y += 0.6f;
                end.y += 0.6f;
                Gizmos.DrawLine(start, end);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(end, 0.1f);
            }
        }
    }

    private Vector3Int TruncateVec3(Vector3 v)
    {
        return new Vector3Int(
            (int)v.x,
            (int)v.y,
            (int)v.z);
    }

    // Provides a normaized direction in the direction needed to seek to a point (World Space)
    // returns whether the agent has arrived
    public bool Seek(Vector3 destination, out Vector2 direction)
    {
        direction = Vector2.zero;
        if (_disableNav || (destination - transform.position).sqrMagnitude <= _arrivalDistance * _arrivalDistance)
        {
            direction = (destination - transform.position).normalized;
            _path = null;
            return true;
        }

        Vector3 destinationLocal = _level.transform.InverseTransformPoint(destination);
        Vector3Int min = TruncateVec3(destinationLocal);
        Vector3Int max = TruncateVec3(destinationLocal + new Vector3(1f, 0f, 1f));

        NavNode destinationNode = null;
        float sqrDist = float.MaxValue;
        Vector3Int testPos = min;

        CheckAll(destinationLocal, min, max, ref sqrDist, testPos, ref destinationNode);

        if (destinationNode == null)
        {
            Debug.Log(name + ": Destination is off the NavGraph");

            // TODO - find closest tile
            direction = (destination - transform.position).normalized;
            _path = null;
            return true;
        }

        // Find our start node
        destinationLocal = _level.transform.InverseTransformPoint(transform.position);
        min = TruncateVec3(destinationLocal);
        max = TruncateVec3(destinationLocal + new Vector3(1f, 0f, 1f));

        NavNode startNode = null;
        sqrDist = float.MaxValue;
        testPos = min;
        CheckAll(destinationLocal, min, max, ref sqrDist, testPos, ref startNode);

        if (startNode == null)
        {
            Debug.LogError(name + " is not on the Graph");
            direction = (destination - transform.position).normalized;
            _path = null;
            return true;
        }

        _path = _navGraph.Dijkstra(startNode, destinationNode);

        if (_path == null)
        {
            Debug.Log("<color=cyan> " + name + ": could not find path");
            direction = (destination - transform.position).normalized;
            _path = null;
            return true;
        }

        Vector3 destWorld = destination;

        if (_path.Count > 1)
        {
            destWorld = _level.transform.TransformPoint(_path[1].coord);
        }
        Vector3 destOffset = destWorld - transform.position;
        direction = new Vector2(destOffset.x, destOffset.z).normalized;

        return false;

    }

    private void CheckAll(Vector3 destinationLocal, Vector3Int min, Vector3Int max, ref float sqrDist, Vector3Int testPos, ref NavNode startNode)
    {
        //Min, Min, Min
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
        //Min, Min, Max
        testPos.z = max.z;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
        //Max, Min, Max
        testPos.x = max.x;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
        //Max, Min, Min
        testPos.z = min.z;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);

        //Min, Max, Min
        testPos.x = min.x;
        testPos.y = min.y;
        testPos.z = min.z;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
        //Min, Max, Max
        testPos.z = max.z;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
        //Max, Max, Max
        testPos.x = max.x;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
        //Max, Max, Min
        testPos.z = min.z;
        CheckCloser(destinationLocal, ref startNode, ref sqrDist, testPos);
    }

    private void CheckCloser(Vector3 destinationLocal, ref NavNode destinationNode, ref float sqrDist, Vector3Int testPos)
    {
        if (!_navGraph.nodes.ContainsKey(testPos))
            return;

        if (_navGraph.nodes[testPos] != null)
        {
            float testDistSqr = (testPos - destinationLocal).sqrMagnitude;
            if (testDistSqr < sqrDist)
            {
                destinationNode = _navGraph.nodes[testPos];
                sqrDist = testDistSqr;
            }
        }
    }
}
