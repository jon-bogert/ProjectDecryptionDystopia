using UnityEngine;
using UnityEngine.Events;
using XephTools;

public class LevelManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] string _filename = "";
    [SerializeField] float _occlusionDelay = 5f;

    [Header("Prefabs")]
    [SerializeField] GameObject _blockPrefab;
    [SerializeField] GameObject _slopePrefab;
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] GameObject _enemyMeleePrefab;
    [SerializeField] GameObject _enemyRangedPrefab;
    [SerializeField] GameObject _doorPrefab;
    [SerializeField] GameObject _keyPrefab;
    [SerializeField] GameObject _buttonPrefab;

    [Header("Events")]
    [SerializeField] UnityEvent onLevelComplete;

    TileMap3D _tileMap = new TileMap3D();
    OccludeCamera _cameraOccluder;
    ValueMover _valueMover;
    SceneLoader _sceneLoader;

    public TileMap3D tileMap { get { return _tileMap; } }
    public string levelName { get { return _tileMap.levelName; } }

    private void Awake()
    {
        _sceneLoader = FindObjectOfType<SceneLoader>();
        if (_sceneLoader == null)
        {
            Debug.LogError("No Scene Loader in current Scene");
        }

        if (_filename != "")
        {
            _tileMap.LoadFromFile(_filename);
        }
        else
        {
            // Make sure SceneLoad is before LevelManager in execultion order
            string filename = _sceneLoader.GetLevelFile();
            _tileMap.LoadFromFile(filename);
        }

        Vector3 offset = ((Vector3)_tileMap.dimensions) * 0.5f;
        transform.position -= offset;

        float y = _sceneLoader.levelY - _tileMap.dimensions.y * 0.5f;
        if (y < 1)
            y = 1;

        transform.position = new Vector3(
            transform.position.x,
            y,
            transform.position.z);

        _tileMap.ForEach(Generate);
        FindObjectOfType<NavGraph>().GenerateGraph(_tileMap);

        _cameraOccluder = FindObjectOfType<OccludeCamera>();
        TimeIt _startTimer = new();
        _startTimer.SetDuration(_occlusionDelay).OnComplete(() => _cameraOccluder.UnBlock()).Start();
    }

    private void Start()
    {
        _valueMover = FindObjectOfType<ValueMover>();
        if (_valueMover == null)
            Debug.LogWarning("No Value Mover in Scene");
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
            case TileType.Button:
                offset = tile.gridCoord;
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                prefab = _buttonPrefab;
                break;
            case TileType.PlayerMovable:
                offset = tile.gridCoord;
                prefab = MovableDictionary.GetPlayerMovable(((PlayerMovableTile)tile).key);
                rotation = ((int)((RotatableTile)tile).rotation) * 90f;
                break;
            case TileType.SelfMovable:
                offset = tile.gridCoord;
                prefab = MovableDictionary.GetSelfMovable(((SelfMovableTile)tile).key);
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
        else if (tile.type == TileType.SelfMovable)
        {
            go.GetComponent<SelfMovable>().Init((SelfMovableTile)tile);
        }
        else if (tile.type == TileType.Button)
        {
            int size = ((ButtonTile)tile).signalId.Length;
            Button btn = go.GetComponent<Button>();
            btn.signalId = new int[size];
            System.Array.Copy(((ButtonTile)tile).signalId, btn.signalId, size);
        }


#if UNITY_EDITOR
        tile.gameObject = go;
#endif // UNITY_EDITOR
    }

    public void LevelComplete()
    {
        onLevelComplete?.Invoke();
    }

    public void SetHeight()
    {
        CharacterController[] controllers = FindObjectsOfType<CharacterController>();
        foreach (CharacterController controller in controllers)
        {
            controller.enabled = false;
        }

        _sceneLoader.MeasureLevelY();

        float y = _sceneLoader.levelY - _tileMap.dimensions.y * 0.5f;
        if (y < 1)
            y = 1;

        Vector3 newPos = new Vector3(
            transform.position.x,
            y,
            transform.position.z);
        
        float delta = newPos.y - transform.position.y;
        transform.position = newPos;

        _valueMover?.afterMoveEvent?.Invoke(delta);

        foreach (CharacterController controller in controllers)
        {
            controller.enabled = true;
        }
    }
}
