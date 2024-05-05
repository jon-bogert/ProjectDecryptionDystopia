using UnityEngine;

public class ButtonTile : RotatableTile
{
    FillerTile _fillerTile = null;
    public FillerTile FillerTile
    {
        get { return _fillerTile; }
        internal set { _fillerTile = value; }
    }

    public ButtonTile()
    {
        base._type = TileType.Button;
    }
}