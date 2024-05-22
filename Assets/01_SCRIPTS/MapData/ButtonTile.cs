using UnityEngine;

public class ButtonTile : RotatableTile
{
    TileBase _fillerTile = null;
    public TileBase FillerTile
    {
        get { return _fillerTile; }
        internal set { _fillerTile = value; }
    }

    public ButtonTile()
    {
        base._type = TileType.Button;
    }
}