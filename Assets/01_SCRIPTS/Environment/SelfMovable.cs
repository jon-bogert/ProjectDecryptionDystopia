using System.Collections.Generic;
using UnityEngine;
using XephTools;

public class SelfMovable : MonoBehaviour
{
    public enum State { Start, End, ToStart, ToEnd };

    [SerializeField] Vector3 _moveAmount = Vector3.zero;
    [SerializeField] bool _isOscillating = false;
    [SerializeField] float _moveTime = 1.5f;
    [SerializeField] float _easeAmount = 0.01f;

    [Space]
    public List<PlayerMovable.FillerDirections> fillerDirections = new();

    public int signalId { get { return _tile.signalID; } }

    State _state = State.Start;
    Vector3 _startPoint = Vector3.zero;
    bool _isRunning = false;
    float _oscTimer = 0f;

    SelfMovableTile _tile = null;
    MovableShaderController _shader;
    PlayerMovableSound _moveSound;
    SoundPlayer3D _soundPlayer;

    public SelfMovableTile tile { get { return _tile; } internal set { _tile = value; } }

    OverTime.ModuleReference<Vector3CubicLerp> _lerpRef;

    private void Start()
    {
        _startPoint = transform.position;
        _moveSound = GetComponent<PlayerMovableSound>();
        if (_moveSound == null)
            Debug.LogError("Player Movable Sound component not found");

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    private void Update()
    {
        if (!_isRunning)
            return;

        float piTimer = (_oscTimer / _moveTime) * Mathf.PI * 2f;
        float t = (-Mathf.Cos(piTimer) * 0.5f) + 0.5f; // 0 -> 1

        transform.position = _startPoint + (t * _moveAmount);

        _oscTimer += Time.deltaTime;
        while (_oscTimer >= _moveTime)
            _oscTimer -= _moveTime;
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(transform.position + _playerCheckCenter, _playerCheckExtends);

        if (_tile == null)
            return;
        LevelManager level = FindObjectOfType<LevelManager>();
        if (level == null)
            return;

        foreach (TileBase filler in _tile.FillerTiles)
        {
            Vector3 pos = level.transform.TransformPoint(filler.gridCoord);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos, Vector3.one * 0.5f);
        }
    }

    public void Init(SelfMovableTile tile)
    {
        _tile = tile;
        _moveAmount = tile.moveAmount;
        _moveTime = tile.moveTime;
        _easeAmount = tile.easeAmount;
        _isOscillating = tile.isOscillating;
        _startPoint = transform.position;
    }

    public void Toggle()
    {
        if (_isOscillating)
        {
            _isRunning = !_isRunning;

            if (_isRunning)
                _moveSound.On();
            else
                _moveSound.Off();

            return;
        }

        _moveSound.On();

        float progress = 0f;
        if (_state == State.ToEnd || _state == State.ToStart)
        {
            progress = _lerpRef.Get().Progress;
            _lerpRef.Get().End();
        }

        if (_state == State.End || _state == State.ToEnd)
        {
            Vector3 startPos = (_state == State.End) ? _startPoint + _moveAmount : transform.position;
            _state = State.ToStart;
            Vector3CubicLerp lerp = new(startPos, _startPoint,
                (progress == 0f) ? _startPoint + _moveAmount * (1f - _easeAmount) : startPos,
                _startPoint + _moveAmount * _easeAmount,
                (progress == 0f) ? _moveTime : _moveTime - ((1 - progress) * _moveTime),
                (val) => { transform.position = val; }
                );
            lerp.OnComplete(() => { _state = State.Start; _moveSound.Off(); });
            _lerpRef = OverTime.Add(lerp);
            return;
        }

        Vector3 startPos2 = (_state == State.Start) ? _startPoint : transform.position;
        _state = State.ToEnd;
        Vector3CubicLerp lerp2 = new(startPos2, _startPoint + _moveAmount,
            (progress == 0f) ? _startPoint + _moveAmount * _easeAmount : startPos2,
            _startPoint + _moveAmount * (1 - _easeAmount),
            (progress == 0f) ? _moveTime : _moveTime - ((1 - progress) * _moveTime),
            (val) => { transform.position = val; }
            );
        lerp2.OnComplete(() => { _state = State.End; _moveSound.Off(); });
        _lerpRef = OverTime.Add(lerp2);
    }
}
