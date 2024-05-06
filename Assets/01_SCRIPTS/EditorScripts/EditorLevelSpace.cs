using UnityEngine;

public class EditorLevelSpace : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject _boundsPrefab;
    [Space]
    [SerializeField] GameObject _blockPrefab;
    [SerializeField] GameObject _slopePrefab;

    EditorOffsetLevelSpace _offsetSpace;

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
        _offsetSpace.transform.position = transform.position + ((Vector3)_tilemap.dimensions * 0.5f);

        GameObject bounds = Instantiate(_boundsPrefab, transform.position, Quaternion.identity, transform);
        bounds.transform.localScale = _tilemap.dimensions;
    }
}
