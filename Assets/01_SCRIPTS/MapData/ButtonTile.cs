using UnityEngine;

public class ButtonTile : RotatableTile
{
    TileBase _fillerTile = null;
    public int[] signalId = new int[1] { 0 };

    public TileBase FillerTile
    {
        get { return _fillerTile; }
        internal set { _fillerTile = value; }
    }

    public ButtonTile()
    {
        base._type = TileType.Button;
    }

    public void SetupTilemap(TileMap3D tilemap)
    {
        _fillerTile = new TileBase();
        _fillerTile.type = TileType.Filler;
        _fillerTile.gridCoord = gridCoord + Vector3Int.up;
        
        tilemap.SetTileAt(_fillerTile.gridCoord, _fillerTile);
        tilemap.SetTileAt(gridCoord, this);
    }
    public void EraseTileMap(TileMap3D tilemap)
    {
        TileBase space = new();
        space.gridCoord = _fillerTile.gridCoord;
        tilemap.SetTileAt(_fillerTile.gridCoord, space);
        _fillerTile = null;

        TileBase tile = new();
        tile.gridCoord = gridCoord;
        tilemap.SetTileAt(gridCoord, tile);
    }
}