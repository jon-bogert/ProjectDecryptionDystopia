using UnityEngine;

public class DoorTile : RotatableTile
{
    //[0][1]
    //[x][2]
    TileBase[] _fillerTiles = null;
    public TileBase[] FillerTiles
    {
        get { return _fillerTiles; }
        internal set { _fillerTiles = value; }
    }

    public DoorTile()
    {
        base._type = TileType.Door;
    }

    public void SetupTilemap(TileMap3D tilemap)
    {
        _fillerTiles = new TileBase[3];
        _fillerTiles[0] = new TileBase();
        _fillerTiles[1] = new TileBase();
        _fillerTiles[2] = new TileBase();

        _fillerTiles[0].type = _fillerTiles[1].type = _fillerTiles[2].type = TileType.Filler;

        _fillerTiles[0].gridCoord = gridCoord + Vector3Int.up;
        Vector3Int side = new();
        switch (rotation)
        {
            case TileRotation.North:
                side = Vector3Int.right;
                break;
            case TileRotation.East:
                side = Vector3Int.back;
                break;
            case TileRotation.South:
                side = Vector3Int.left;
                break;
            default:
                side = Vector3Int.forward;
                break;
        }

        _fillerTiles[1].gridCoord = gridCoord + side + Vector3Int.up;
        _fillerTiles[2].gridCoord = gridCoord + side;

        for (int i = 0; i < _fillerTiles.Length; ++i)
        {
            tilemap.SetTileAt(_fillerTiles[i].gridCoord, _fillerTiles[i]);
        }

        tilemap.SetTileAt(gridCoord, this);
    }

    public void EraseTileMap(TileMap3D tilemap)
    {
        for (int i = 0; i < _fillerTiles.Length; ++i)
        {
            TileBase space = new();
            space.gridCoord = _fillerTiles[i].gridCoord;
            tilemap.SetTileAt(_fillerTiles[i].gridCoord, space);
        }
        _fillerTiles = null;

        TileBase tile = new();
        tile.gridCoord = gridCoord;
        tilemap.SetTileAt(gridCoord, tile);
    }
}