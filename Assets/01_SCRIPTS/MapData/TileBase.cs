using UnityEngine;

public enum TileType
{
    Filler = -2,
    PlayerStart = -1,
    Space = 0,
    Block = 1,
    Slope,
    Door,
    Button,
    SelfMovable,
    UserMovable,
}

public enum TileRotation // the opposite face of the tile when you approach it
{
    North,
    East,
    South,
    West
}

public class TileBase
{
    protected TileType _type = TileType.Space;
    protected Vector3Int _gridCoord = Vector3Int.zero;
    public TileType type { get { return _type; }
        internal set { _type = value; } }
    public Vector3Int gridCoord { get { return _gridCoord; }
        internal set { _gridCoord = value; } }

}
