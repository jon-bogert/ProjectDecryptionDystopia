using UnityEngine;

public class RotatableTile : TileBase
{
    TileRotation _rotation = TileRotation.North;

    public TileRotation Rotation
    {
        get { return _rotation; }
        internal set { _rotation = value; }
    }
}
