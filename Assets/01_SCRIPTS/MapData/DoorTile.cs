using UnityEngine;

public class DoorTile : RotatableTile
{
    FillerTile[] _fillerTiles = null;
    public FillerTile[] FillerTiles
    {
        get { return _fillerTiles; }
        internal set { _fillerTiles = value; }
    }

    public DoorTile()
    {
        base._id = TileType.Door;
    }
}