﻿using UnityEngine;

public class EditorOffsetLevelSpace : MonoBehaviour
{
    [SerializeField] GameObject _cursor;
    [SerializeField] Transform _playerHand;

    [Header("References")]
    [SerializeField] GameObject _blockPrefab;
    [SerializeField] GameObject _slopePrefab;
    [SerializeField] GameObject _playerStartPrefab;
    [SerializeField] GameObject _enemyMeleePrefab;
    [SerializeField] GameObject _enemyRangedPrefab;
    [SerializeField] GameObject _doorPrefab;
    [SerializeField] GameObject _keyPrefab;
    [SerializeField] GameObject _buttonPrefab;

    GameObject[] _visuals = null;

    public Vector3Int cursorPosition
    {
        get
        {
            Vector3 handRelative = transform.InverseTransformPoint(_playerHand.transform.position);
            int x = (int)Mathf.Round(handRelative.x);
            int y = (int)Mathf.Round(handRelative.y);
            int z = (int)Mathf.Round(handRelative.z);
            return new Vector3Int(x, y, z);
        }
    }

    public bool isInBounds
    {
        get
        {
            Vector3Int cur = cursorPosition;
            return (cur.x >= 0 && cur.x < _parentSpace.dimensions.x
            && cur.y >= 0 && cur.y < _parentSpace.dimensions.y
            && cur.z >= 0 && cur.z < _parentSpace.dimensions.z);
        }
    }

    EditorLevelSpace _parentSpace;
    string Vec3Str(Vector3 vec)
    {
        return vec.x.ToString("F2") + " " + vec.y.ToString("F2") + " " + vec.z.ToString("F2");
    }
    private void Update()
    {
        Vector3 cur = _cursor.transform.position;

        if (!_parentSpace.hasValidTilemap)
            return;

        Vector3 handRelative = transform.InverseTransformPoint(_playerHand.transform.position);
        handRelative.x = Mathf.Round(handRelative.x);
        handRelative.y = Mathf.Round(handRelative.y);
        handRelative.z = Mathf.Round(handRelative.z);

        bool isOutBounds =
            (handRelative.x < 0 || handRelative.x >= _parentSpace.dimensions.x
            || handRelative.y < 0 || handRelative.y >= _parentSpace.dimensions.y
            || handRelative.z < 0 || handRelative.z >= _parentSpace.dimensions.z);

        if (isOutBounds && _cursor.activeSelf)
            _cursor.SetActive(false);
        else if (!isOutBounds && !_cursor.activeSelf)
            _cursor.SetActive(true);

        if (isOutBounds)
            return;

        _cursor.transform.position = transform.TransformPoint(handRelative);
    }

    public void SetParentSpace(EditorLevelSpace parentSpace)
    {
        _parentSpace = parentSpace;
    }

    public void SetVisualSize(Vector3Int dimensions)
    {
        _visuals = new GameObject[dimensions.x * dimensions.y * dimensions.z];
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    int CoordToIndex(Vector3Int coord)
    {
        return coord.z * _parentSpace.tilemap.dimensions.y * _parentSpace.tilemap.dimensions.x
            + coord.y * _parentSpace.tilemap.dimensions.x
            + coord.x;
    }

    public void GenerateVisual()
    {
        Vector3Int coord = cursorPosition;
        int index = CoordToIndex(coord);

        if (_visuals[index] != null)
        {
            Destroy(_visuals[index]);
            _visuals[index] = null;
        }
        TileBase tile = _parentSpace.tilemap.TileAt(coord);
        if (tile.type == TileType.Space ||
            tile.type == TileType.Filler)
            return;

        if (tile.type == TileType.Block)
        {
            _visuals[index] = Instantiate(_blockPrefab,
                transform.TransformPoint(coord),
                Quaternion.identity,
                transform);
        }
        else if (tile.type == TileType.Slope)
        {
            RotatableTile rt = (RotatableTile)tile;
            _visuals[index] = Instantiate(_slopePrefab,
                transform.TransformPoint(coord),
                Quaternion.Euler(0f, ((float)rt.rotation) * 90f, 0f),
                transform);
        }
        else if (tile.type == TileType.PlayerStart)
        {
            _visuals[index] = Instantiate(_playerStartPrefab,
                transform.TransformPoint(coord),
                Quaternion.identity,
                transform);
        }
        else if (tile.type == TileType.EnemyMelee)
        {
            RotatableTile rt = (RotatableTile)tile;
            _visuals[index] = Instantiate(_enemyMeleePrefab,
                transform.TransformPoint(coord),
                Quaternion.Euler(0f, ((float)rt.rotation) * 90f, 0f),
                transform);
        }
        else if (tile.type == TileType.EnemyRanged)
        {
            RotatableTile rt = (RotatableTile)tile;
            _visuals[index] = Instantiate(_enemyRangedPrefab,
                transform.TransformPoint(coord),
                Quaternion.Euler(0f, ((float)rt.rotation) * 90f, 0f),
                transform);
        }
        else if (tile.type == TileType.Door)
        {
            RotatableTile rt = (RotatableTile)tile;
            _visuals[index] = Instantiate(_doorPrefab,
                transform.TransformPoint(coord),
                Quaternion.Euler(0f, ((float)rt.rotation) * 90f, 0f),
                transform);
        }
        else if (tile.type == TileType.Key)
        {
            _visuals[index] = Instantiate(_keyPrefab,
                transform.TransformPoint(coord),
                Quaternion.identity,
                transform);
        }
        else if (tile.type == TileType.Button)
        {
            RotatableTile rt = (RotatableTile)tile;
            _visuals[index] = Instantiate(_buttonPrefab,
                transform.TransformPoint(coord),
                Quaternion.Euler(0f, ((float)rt.rotation) * 90f, 0f),
                transform);
        }
    }
}
