using UnityEngine;
using UnityEngine.WSA;

public class EditorLevelSpace : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject _boundsPrefab;

    EditorOffsetLevelSpace _offsetSpace;
    GameObject _activeBounds = null;

    public Vector3Int cursorPosition { get { return _offsetSpace.cursorPosition; } }

    private void Awake()
    {
        _offsetSpace = GetComponentInChildren<EditorOffsetLevelSpace>();
        _offsetSpace.SetParentSpace(this);
    }

    public TileMap3D tilemap
    {
        get
        {
            return _tilemap;
        }
        set
        {
            _tilemap = value;
            Generate();
        }
    }

    public bool hasValidTilemap { get { return _tilemap != null; } }
    public Vector3Int dimensions {  get { return _tilemap.dimensions; } } 

    TileMap3D _tilemap = null;

    public void SetTilemap(TileMap3D tilemap)
    {
        _tilemap = tilemap;
        Generate();
    }

    private void Generate()
    {
        _offsetSpace.Clear();
        _offsetSpace.SetVisualSize(_tilemap.dimensions);
        _offsetSpace.transform.position = transform.position - ((Vector3)_tilemap.dimensions * 0.5f);

        if (_activeBounds)
            Destroy(_activeBounds);

        _activeBounds = Instantiate(_boundsPrefab,
            transform.position - (Vector3.one * 0.5f),
            Quaternion.identity, transform);

        _activeBounds.transform.localScale = _tilemap.dimensions;
    }

    public void Place(TileType tileType, TileRotation rotation)
    {
        if (!_offsetSpace.isInBounds)
            return;

        if (tileType == _tilemap.TileAt(cursorPosition).type)
        {
            return;
        }

        if (tileType == TileType.Block)
        {
            TileBase tile = new TileBase();
            tile.type = tileType;
            _tilemap.SetTileAt(cursorPosition, tile);
        }
        else if (tileType == TileType.Slope)
        {
            RotatableTile tile = new RotatableTile();
            tile.type = tileType;
            tile.rotation = rotation;
            _tilemap.SetTileAt(cursorPosition, tile);
        }
        else if (tileType == TileType.PlayerStart)
        {
            TileBase tile = new TileBase();
            tile.type = tileType;
            _tilemap.SetTileAt(cursorPosition, tile);
        }
        else if (tileType == TileType.EnemyMelee || tileType == TileType.EnemyRanged)
        {
            RotatableTile tile = new RotatableTile();
            tile.type = tileType;
            tile.rotation = rotation;
            _tilemap.SetTileAt(cursorPosition, tile);
        }
        else if (tileType == TileType.Key)
        {
            TileBase tile = new TileBase();
            tile.type = tileType;
            _tilemap.SetTileAt(cursorPosition, tile);
        }
        else if (tileType == TileType.Door)
        {
            DoorTile tile = new DoorTile();
            tile.type = tileType;
            tile.gridCoord = cursorPosition;
            tile.rotation = rotation;
            tile.SetupTilemap(_tilemap);
        }
        else if (tileType == TileType.Button)
        {
            ButtonTile tile = new ButtonTile();
            tile.type = tileType;
            tile.gridCoord = cursorPosition;
            tile.rotation = rotation;
            tile.SetupTilemap(_tilemap);
        }
        else
        {
            Debug.LogError("Unimplemented Enum");
        }

        _offsetSpace.GenerateVisual();
    }

    public void Erase()
    {
        if (!_offsetSpace.isInBounds)
            return;

        TileBase tile = _tilemap.TileAt(cursorPosition);
        TileType currType = tile.type;
        if (currType == TileType.Space || currType == TileType.Filler)
            return;

        if (currType == TileType.Door)
        {
            DoorTile door = tile as DoorTile;
            door.EraseTileMap(_tilemap);
            _offsetSpace.GenerateVisual();
            return;
        }
        else if (currType == TileType.Button)
        {
            ButtonTile button = tile as ButtonTile;
            button.EraseTileMap(_tilemap);
            _offsetSpace.GenerateVisual();
            return;
        }

        tile = new TileBase();
        tile.type = TileType.Space;
        _tilemap.SetTileAt(cursorPosition, tile);

        _offsetSpace.GenerateVisual();
    }
}
