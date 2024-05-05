using UnityEngine;

public class FillerTile : TileBase
{
    public FillerTile()
    {
        base._type = TileType.Filler;
    }

    TileBase _association = null;
    public TileBase association
    {
        get { return _association; }
        internal set { _association = value; }
    }
}
