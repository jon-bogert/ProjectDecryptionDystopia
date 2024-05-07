using UnityEngine;
using UnityEngine.InputSystem;

public class EditorBlockPlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _controllerPoint;
    [Space]
    [SerializeField] GameObject _blockObj;
    [SerializeField] GameObject _slopeObj;

    [Header("Inputs")]
    [SerializeField] InputActionReference _placeInput;
    [SerializeField] InputActionReference _eraseInput;
    [SerializeField] InputActionReference _nextInput;
    [SerializeField] InputActionReference _prevInput;

    TileType _tileType = TileType.Block;
    EditorLevelSpace _levelSpace;
    TileRotation _rotation;

    private void Awake()
    {
        _prevInput.action.performed += PrevInput;
        _nextInput.action.performed += NextInput;
    }

    private void Start()
    {
        UpdateVisual();

        _levelSpace = FindObjectOfType<EditorLevelSpace>();
    }

    private void Update()
    {
        if (_placeInput.action.IsPressed())
        {
            _levelSpace.Place(_tileType, _rotation);
        }
        else if (_eraseInput.action.IsPressed())
        {
            _levelSpace.Erase();
        }
    }

    private void OnDestroy()
    {
        _prevInput.action.performed -= PrevInput;
        _nextInput.action.performed -= NextInput;
    }

    void UpdateVisual()
    {
        _blockObj.SetActive(false);
        _slopeObj.SetActive(false);

        switch (_tileType)
        {
            case TileType.Block:
                _blockObj.SetActive(true);
                break;
            case TileType.Slope:
                _slopeObj.SetActive(true);
                break;
            default:
                Debug.LogError("Unimplemented Enum");
                break;
        }
    }

    private void PrevInput(InputAction.CallbackContext ctx)
    {
        if (_tileType == TileType.Block)
            _tileType = TileType.Button;
        else
            _tileType--;

        UpdateVisual();
    }

    private void NextInput(InputAction.CallbackContext ctx)
    {
        if (_tileType == TileType.Button)
            _tileType = TileType.Block;
        else
            _tileType++;

        UpdateVisual();
    }
}
