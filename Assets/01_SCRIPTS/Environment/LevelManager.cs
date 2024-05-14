using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] string _filename = "";
    [SerializeField] GameObject _blockPrefab;
    [SerializeField] GameObject _slopePrefab;

    TileMap3D _tileMap = new TileMap3D();

    public TileMap3D tileMap { get { return _tileMap; } }

    private void Awake()
    {
        _tileMap.LoadFromFile(_filename);
        Vector3 offset = ((Vector3)_tileMap.dimensions) * 0.5f;
        transform.position -= offset;
        _tileMap.ForEach(Generate);
        FindObjectOfType<NavGraph>().GenerateGraph(_tileMap);
    }

    private void Generate(TileBase tile)
    {
        float rotation = 0f;
        Vector3 offset = Vector3.zero;
        GameObject prefab = null;
        switch (tile.type)
        {
            case TileType.Space:
            case TileType.PlayerStart:
                return;
            case TileType.Block:
                offset = tile.gridCoord;
                prefab = _blockPrefab;
                break;
            case TileType.Slope:
                offset = tile.gridCoord;
                rotation = ((int)((RotatableTile)tile).Rotation) * 90f;
                prefab = _slopePrefab;
                break;
            default:
                Debug.LogError("Unimplemented enum type");
                return;
        }
        GameObject go = Instantiate(prefab, transform.position + offset, Quaternion.Euler(0f, rotation, 0f), this.transform);
#if UNITY_EDITOR
        tile.gameObject = go;
#endif // UNITY_EDITOR
    }
}
