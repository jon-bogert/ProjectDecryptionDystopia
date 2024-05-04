using System.Globalization;
using System.IO;
using UnityEngine;

public class TileMap3D
{
    Vector3Int _dimensions = Vector3Int.zero;
    TileBase[] _tiles = null; // X, then Y, Then Z
    TileBase _playerStart = null; // Set on load;

    public TileBase playerStart { get { return _playerStart; } }

    public TileBase TileAt(Vector3Int coord)
    {
        return TileAt(coord.x, coord.y, coord.z);
    }

    public TileBase TileAt(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0
            || x >= _dimensions.x || y >= _dimensions.y || z >= _dimensions.z)
        {
            Debug.LogError("TileAt coordinates outside of tilemap");
            return null;
        }
        int index = z * _dimensions.y * _dimensions.x
            + y * _dimensions.x
            + x;

        return _tiles[index];
    }

    public void LoadFromFile(string file)
    {
        if (!File.Exists(file))
        {
            Debug.LogError("Could not find file: " + file);
            return;
        }

        string contents = File.ReadAllText(file);
        LoadFromString(contents);
    }

    public void LoadFromString(string contents)
    {
        Debug.Log("TODO - Implement Reading from string");
    }

}