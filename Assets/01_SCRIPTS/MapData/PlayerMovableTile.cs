using UnityEngine;
using System.Collections.Generic;

public class PlayerMovableTile : RotatableTile
{
    List<TileBase> _fillerTiles = new();
    public Vector3 moveAmount = Vector3.zero;
    public string key;

    public List<TileBase> FillerTiles
    {
        get { return _fillerTiles; }
        internal set { _fillerTiles = value; }
    }

    public PlayerMovableTile()
    {
        base._type = TileType.PlayerMovable;
    }

    public void SetupTilemap(string key, TileMap3D tilemap)
    {
        this.key = key;
        PlayerMovable prefab = MovableDictionary.GetPlayerMovable(key).GetComponent<PlayerMovable>();

        List<PlayerMovable.FillerDirections> directions = prefab.fillerDirections;

        Vector3Int side = new();
        Vector3Int frwd = new();

        switch (rotation)
        {
            case TileRotation.North:
                side = Vector3Int.right;
                frwd = Vector3Int.forward;
                break;
            case TileRotation.East:
                side = Vector3Int.back;
                frwd = Vector3Int.right;
                break;
            case TileRotation.South:
                side = Vector3Int.left;
                frwd = Vector3Int.back;
                break;
            default:
                side = Vector3Int.forward;
                frwd = Vector3Int.left;
                break;
        }

        Vector3Int cursor = gridCoord;
        TileBase filler = new();
        foreach (PlayerMovable.FillerDirections dir in directions)
        {
            switch (dir)
            {
                case PlayerMovable.FillerDirections.Left:
                    cursor -= side;
                    break;
                case PlayerMovable.FillerDirections.Right:
                    cursor += side;
                    break;
                case PlayerMovable.FillerDirections.Up:
                    cursor += Vector3Int.up;
                    break;
                case PlayerMovable.FillerDirections.Down:
                    cursor += Vector3Int.down;
                    break;
                case PlayerMovable.FillerDirections.Forward:
                    cursor += frwd;
                    break;
                case PlayerMovable.FillerDirections.Backward:
                    cursor -= frwd;
                    break;
                default: // Stamp
                    filler = new();
                    filler.gridCoord = cursor;
                    filler.type = TileType.Filler;
                    tilemap.SetTileAt(cursor, filler);
                    _fillerTiles.Add(filler);
                    break;
            }
        }
        tilemap.SetTileAt(gridCoord, this);
    }
}
