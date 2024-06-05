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

    [Header("Player Check")]
    [SerializeField] Vector3 _playerCheckCenter = Vector3.zero;
    [SerializeField] Vector3 _playerCheckExtends = Vector3.one;
    [SerializeField] LayerMask _playerCheckMask = 0;

    [Space]
    public List<PlayerMovable.FillerDirections> fillerDirections = new();

    public int signalId { get { return _tile.signalID; } }

    State _state = State.Start;
    Vector3 _startPoint = Vector3.zero;
    bool _isRunning = false;
    float _oscTimer = 0f;

    SelfMovableTile _tile = null;
    MovableShaderController _shader;
    SoundPlayer3D _soundPlayer;
    ThirdPersonMovement _player = null;
    ValueMover _valueMover;

    public SelfMovableTile tile { get { return _tile; } internal set { _tile = value; } }

    OverTime.ModuleReference<Vector3CubicLerp> _lerpRef;

    private void Start()
    {
        _startPoint = transform.position;

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");

        _valueMover = FindObjectOfType<ValueMover>();
        if (_valueMover)
        {
            _valueMover.afterMoveEvent += MoveValues;
        }
    }

    private void Update()
    {
        CheckPlayer();

        if (!_isRunning)
            return;

        float piTimer = (_oscTimer / _moveTime) * Mathf.PI * 2f;
        float t = (-Mathf.Cos(piTimer) * 0.5f) + 0.5f; // 0 -> 1

        Vector3 newPos  = _startPoint + (t * _moveAmount);
        MovePlayer(newPos - transform.position);
        transform.position = newPos;

        _oscTimer += Time.deltaTime;
        while (_oscTimer >= _moveTime)
            _oscTimer -= _moveTime;
    }

    private void OnDestroy()
    {
        if (_valueMover)
        {
            _valueMover.afterMoveEvent -= MoveValues;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + _playerCheckCenter, _playerCheckExtends);

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
            return;
        }


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
                (val) => { MovePlayer(val - transform.position); transform.position = val; }
                );
            lerp.OnComplete(() => { _state = State.Start; });
            _lerpRef = OverTime.Add(lerp);
            return;
        }

        Vector3 startPos2 = (_state == State.Start) ? _startPoint : transform.position;
        _state = State.ToEnd;
        Vector3CubicLerp lerp2 = new(startPos2, _startPoint + _moveAmount,
            (progress == 0f) ? _startPoint + _moveAmount * _easeAmount : startPos2,
            _startPoint + _moveAmount * (1 - _easeAmount),
            (progress == 0f) ? _moveTime : _moveTime - ((1 - progress) * _moveTime),
            (val) => { MovePlayer(val - transform.position); transform.position = val; }
            );
        lerp2.OnComplete(() => { _state = State.End; });
        _lerpRef = OverTime.Add(lerp2);
    }

    private void MovePlayer(Vector3 amt)
    {
        if (_player == null)
            return;

        _player.Move(amt);
    }

    private void CheckPlayer()
    {
        Collider[] collisions = Physics.OverlapBox(transform.position + _playerCheckCenter, _playerCheckExtends * 0.5f, Quaternion.identity, _playerCheckMask);
        if (collisions.Length <= 0)
        {
            if (_player != null)
            {
                _player = null;
            }
            return;
        }

        if (_player != null)
            return;

        foreach (Collider collider in collisions)
        {
            _player = collider.GetComponent<ThirdPersonMovement>();
            if (_player != null)
            {
                return;
            }
        }
    }

    private void MoveValues(float amount)
    {
        _startPoint += Vector3.up * amount;
    }
}
