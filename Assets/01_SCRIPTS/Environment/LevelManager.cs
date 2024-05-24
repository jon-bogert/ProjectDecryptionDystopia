using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] string _filename = "";

    [Header("Prefabs")]
    [SerializeField] GameObject _blockPrefab;
    [SerializeField] GameObject _slopePrefab;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] GameObject _enemyMeleePrefab;
    [SerializeField] GameObject _enemyRangedPrefab;
    [SerializeField] GameObject _doorPrefab;
    [SerializeField] GameObject _keyPrefab;

    [Header("Events")]
    [SerializeField] UnityEvent onLevelComplete;

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
            case TileType.Filler:
                return;
            case TileType.Block:
                offset = tile.gridCoord;
                prefab = _blockPrefab;
                break;
            case TileType.Slope:
                offset = tile.gridCoord;
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                prefab = _slopePrefab;
                break;
            case TileType.PlayerStart:
                offset = tile.gridCoord;
                prefab = _playerPrefab;
                break;
            case TileType.EnemyMelee:
                offset = tile.gridCoord;
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                prefab = _enemyMeleePrefab;
                break;
            case TileType.EnemyRanged:
                offset = tile.gridCoord;
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                prefab = _enemyRangedPrefab;
                break;
            case TileType.Door:
                offset = tile.gridCoord;
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                prefab = _doorPrefab;
                break;
            case TileType.Key:
                offset = tile.gridCoord;
                prefab = _keyPrefab;
                break;
            case TileType.PlayerMovable:
                offset = tile.gridCoord;
                prefab = MovableDictionary.GetPlayerMovable(((PlayerMovableTile)tile).key);
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                break;
            default:
                Debug.LogError("Unimplemented enum type");
                return;
        }
        GameObject go = Instantiate(prefab, transform.position + offset, Quaternion.Euler(0f, rotation, 0f), this.transform);

        if (tile.type == TileType.PlayerMovable)
        {
            go.GetComponent<PlayerMovable>().Init((PlayerMovableTile)tile);
        }

#if UNITY_EDITOR
        tile.gameObject = go;
#endif // UNITY_EDITOR
    }

    public void LevelComplete()
    {
        Debug.Log("<color=green>Level Complete!");
        onLevelComplete?.Invoke();
    }
}
