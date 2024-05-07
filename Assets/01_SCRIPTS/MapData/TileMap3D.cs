using System.IO;
using UnityEngine;
using YamlDotNet.RepresentationModel;

public class TileMap3D
{
    public delegate void Visitor(TileBase tile);

    Vector3Int _dimensions = Vector3Int.zero;
    TileBase[] _tiles = null; // X, then Y, Then Z
    TileBase _playerStart = null; // Set on load;

    public TileBase playerStart { get { return _playerStart; } }
    public Vector3Int dimensions { get { return _dimensions; } }

    public void ForEach(Visitor visitor)
    {
        for (int i = 0; i < _tiles.Length; ++i)
        {
            visitor?.Invoke(_tiles[i]);
        }
    }

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

    //Only for use in Tilemap Editor
    internal void SetTileAt(Vector3Int coord, TileBase tile)
    {
        if (coord.x < 0 || coord.y < 0 || coord.z < 0
            || coord.x >= _dimensions.x || coord.y >= _dimensions.y || coord.z >= _dimensions.z)
        {
            Debug.LogError("TileAt coordinates outside of tilemap");
            return;
        }

        int index = coord.z * _dimensions.y * _dimensions.x
            + coord.y * _dimensions.x
            + coord.x;

        tile.gridCoord = coord;
        _tiles[index] = tile;
    }

    Vector3Int GetCoordFromIndex(int index)
    {
        return new Vector3Int(
                index % _dimensions.x,
                (index / _dimensions.x) % _dimensions.y,
                index / (_dimensions.x * _dimensions.y));
    }

    int GetIndexFromCoord(Vector3Int coord)
    {
        return  coord.z * _dimensions.y * _dimensions.x
            + coord.y * _dimensions.x
            + coord.x;
    }

    public void CreateEmpty(Vector3Int dimensions)
    {
        _dimensions = dimensions;
        int length = _dimensions.x * _dimensions.y * _dimensions.z;
        _tiles = new TileBase[length];
        for (int i = 0; i < length; ++i)
        {
            _tiles[i] = new TileBase();
            _tiles[i].type = TileType.Space;
            _tiles[i].gridCoord = GetCoordFromIndex(i);
        }
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
        YamlStream yamlStream = new YamlStream();
        using (TextReader reader = new StringReader(contents))
        {
            yamlStream.Load(reader);
        }

        YamlMappingNode root = (YamlMappingNode)yamlStream.Documents[0].RootNode;

        _dimensions.x = int.Parse(root["dimensions"]["x"].ToString());
        _dimensions.y = int.Parse(root["dimensions"]["y"].ToString());
        _dimensions.z = int.Parse(root["dimensions"]["z"].ToString());

        int length = _dimensions.x * _dimensions.y * _dimensions.z;

        _tiles = new TileBase[length];
        for (int i = 0; i < length; ++i)
        {
            _tiles[i] = new TileBase();
            _tiles[i].gridCoord = GetCoordFromIndex(i);
        }

        YamlSequenceNode tiles = (YamlSequenceNode)root["tiles"];
        for (int i = 0; i < tiles.Children.Count; ++i)
        {
            YamlMappingNode tileData = (YamlMappingNode)tiles.Children[i];
            Vector3Int coord = Vector3Int.zero;
            coord.x = int.Parse(tileData["coord"]["x"].ToString());
            coord.y = int.Parse(tileData["coord"]["y"].ToString());
            coord.z = int.Parse(tileData["coord"]["z"].ToString());

            TileBase currTile = TileAt(coord);
            if (!System.Enum.TryParse<TileType>(tileData["type"].ToString(), out TileType type))
            {
                Debug.LogError("Bad tile type, could not parse to enum: " + tileData["type"].ToString());
                continue;
            }

            if (type == TileType.Block)
                currTile.type = type;

            else if (type == TileType.Slope)
            {
                currTile = _tiles[GetIndexFromCoord(coord)] = new RotatableTile();
                currTile.type = TileType.Slope;
                currTile.gridCoord = coord;
                RotatableTile rt = currTile as RotatableTile;
                rt.Rotation = System.Enum.Parse<TileRotation>(tileData["rotation"].ToString());
            }
            else
            {
                Debug.LogWarning("Unimplemented enum type");
            }
        }

    }
}