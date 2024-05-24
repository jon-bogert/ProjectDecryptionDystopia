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
            //Coordinates are outside of the map;
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

            if (type == TileType.Block ||
                type == TileType.Key)
            {
                currTile.type = type;
            }

            else if (type == TileType.PlayerStart)
            {
                currTile.type = type;
                _playerStart = currTile;
            }

            else if (type == TileType.Slope ||
                type == TileType.EnemyMelee ||
                type == TileType.EnemyRanged)
            {
                currTile = _tiles[GetIndexFromCoord(coord)] = new RotatableTile();
                currTile.type = type;
                currTile.gridCoord = coord;
                RotatableTile rt = currTile as RotatableTile;
                rt.rotation = System.Enum.Parse<TileRotation>(tileData["rotation"].ToString());
            }

            else if (type == TileType.Door)
            {
                currTile = _tiles[GetIndexFromCoord(coord)] = new DoorTile();
                DoorTile door = currTile as DoorTile;
                door.type = type;
                door.gridCoord = coord;
                door.rotation = System.Enum.Parse<TileRotation>(tileData["rotation"].ToString());
                door.SetupTilemap(this);
            }

            else if (type == TileType.PlayerMovable)
            {
                currTile = _tiles[GetIndexFromCoord(coord)] = new PlayerMovableTile();
                PlayerMovableTile movable = currTile as PlayerMovableTile;
                movable.type = type;
                movable.gridCoord = coord;
                movable.rotation = System.Enum.Parse<TileRotation>(tileData["rotation"].ToString());
                Vector3 moveAmount = new Vector3(
                    float.Parse(tileData["move-amount"]["x"].ToString()),
                    float.Parse(tileData["move-amount"]["y"].ToString()),
                    float.Parse(tileData["move-amount"]["z"].ToString()));
                movable.moveAmount = moveAmount;
                string key = tileData["key"].ToString();
                movable.SetupTilemap(key, this);
            }

            else
            {
                Debug.LogWarning("Unimplemented enum type");
            }
        }

    }

    //Only for use in Tilemap Editor
    public void SaveToFile(string filename)
    {
        using (StreamWriter file = File.CreateText(filename))
        {
            file.Write(SaveToString());
        }
    }

    //Only for use in Tilemap Editor
    public string SaveToString()
    {
        YamlMappingNode root = new();

        YamlMappingNode dims = new()
        {
            { "x", _dimensions.x.ToString() },
            { "y", _dimensions.y.ToString() },
            { "z", _dimensions.z.ToString() }
        };

        root.Add("dimensions", dims);

        YamlSequenceNode tiles = new();
        foreach (TileBase tileData in _tiles)
        {
            if (tileData.type == TileType.Space ||
                tileData.type == TileType.Filler)
                continue;

            YamlMappingNode tileNode = new();
            tileNode.Add("type", tileData.type.ToString());
            YamlMappingNode coord = new()
            {
                { "x", tileData.gridCoord.x.ToString() },
                { "y", tileData.gridCoord.y.ToString() },
                { "z", tileData.gridCoord.z.ToString() }
            };
            tileNode.Add("coord", coord);
            if (tileData.type == TileType.Slope ||
                tileData.type == TileType.Door ||
                tileData.type == TileType.Button ||
                tileData.type == TileType.EnemyMelee ||
                tileData.type == TileType.EnemyRanged)
            {
                RotatableTile rt = tileData as RotatableTile;
                tileNode.Add("rotation", rt.rotation.ToString());
            }
            tiles.Add(tileNode);
        }
        root.Add("tiles", tiles);

        string result = "";
        YamlStream yaml = new();
        yaml.Add(new YamlDocument(root));
        using (StringWriter writer = new())
        {
            yaml.Save(writer, false);
            result = writer.ToString();
            result = result.Substring(0, result.Length - 5); // removing the "..." at end of document
        }

        return result;
    }
}