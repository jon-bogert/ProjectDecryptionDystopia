using UnityEngine;
using UnityEngine.InputSystem;
using XephTools;

public class EditorBlockPlacer : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _rotateUpTreshold = 0.8f;
    [SerializeField] float _rotateDownTreshold = 0.5f;

    [Header("References")]
    [SerializeField] Transform _controllerPoint;
    [Space]
    [SerializeField] GameObject _blockObj;
    [SerializeField] GameObject _slopeObj;
    [SerializeField] GameObject _playerStartObj;
    [SerializeField] GameObject _enemyMeleeObj;
    [SerializeField] GameObject _enemyRangedObj;

    [Header("Inputs")]
    [SerializeField] InputActionReference _placeInput;
    [SerializeField] InputActionReference _eraseInput;
    [SerializeField] InputActionReference _nextInput;
    [SerializeField] InputActionReference _prevInput;
    [SerializeField] InputActionReference _rotateInput;

    TileType _tileType = TileType.Block;
    EditorLevelSpace _levelSpace;
    TileRotation _rotation;

    enum Rotate { None, CW, CCW }
    Rotate _rotate = Rotate.None;

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

        CheckRotation();
        
    }

    private void OnDestroy()
    {
        _prevInput.action.performed -= PrevInput;
        _nextInput.action.performed -= NextInput;
    }

    private void CheckRotation()
    {
        float rotateAxis = _rotateInput.action.ReadValue<Vector2>().x;
        VRDebug.Monitor(2, rotateAxis.ToString("F2"));

        if (_rotate != Rotate.CW && rotateAxis >= _rotateUpTreshold)
        {
            _rotate = Rotate.CW;
            _rotation = (TileRotation)((int)(_rotation + 1)%4);
            UpdateVisual();
        }
        else if (_rotate != Rotate.CCW && rotateAxis <= -_rotateUpTreshold)
        {
            _rotate = Rotate.CCW;
            _rotation = (TileRotation)((int)(_rotation + 3) % 4);
            UpdateVisual();
        }
        else if (_rotate != Rotate.None &&
            rotateAxis <= _rotateDownTreshold &&
            rotateAxis >= -_rotateDownTreshold)
        {
            _rotate = Rotate.None;
        }
    }

    void UpdateVisual()
    {
        _blockObj.SetActive(false);
        _slopeObj.SetActive(false);
        _playerStartObj.SetActive(false);
        _enemyMeleeObj.SetActive(false);
        _enemyRangedObj.SetActive(false);

        switch (_tileType)
        {
            case TileType.Block:
                _blockObj.transform.localRotation = Quaternion.identity;
                _blockObj.SetActive(true);
                break;
            case TileType.Slope:
                _slopeObj.transform.localRotation = Quaternion.Euler(0f, (float)_rotation * 90f, 0f);
                _slopeObj.SetActive(true);
                break;
            case TileType.PlayerStart:
                _playerStartObj.transform.localRotation = Quaternion.identity;
                _playerStartObj.SetActive(true);
                break;
            case TileType.EnemyMelee:
                _enemyMeleeObj.transform.localRotation = Quaternion.Euler(0f, (float)_rotation * 90f, 0f);
                _enemyMeleeObj.SetActive(true);
                break;
            case TileType.EnemyRanged:
                _enemyRangedObj.transform.localRotation = Quaternion.Euler(0f, (float)_rotation * 90f, 0f);
                _enemyRangedObj.SetActive(true);
                break;
            default:
                Debug.LogError("Unimplemented Enum");
                break;
        }
    }

    private void PrevInput(InputAction.CallbackContext ctx)
    {
        if (_tileType == TileType.Block)
            _tileType = TileType.EnemyRanged;
        else
            _tileType--;

        UpdateVisual();
    }

    private void NextInput(InputAction.CallbackContext ctx)
    {
        if (_tileType == TileType.EnemyRanged)
            _tileType = TileType.Block;
        else
            _tileType++;

        UpdateVisual();
    }
}
