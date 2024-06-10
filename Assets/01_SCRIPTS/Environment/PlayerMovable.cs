using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovable : MonoBehaviour
{
    public enum FillerDirections
    {
        Stamp,
        Left,
        Right,
        Up,
        Down,
        Forward,
        Backward,
    }

    [SerializeField] Vector3 _moveAmount = Vector3.zero;
    [Tooltip("For Tutorials where Movable is placed in scene")]
    [SerializeField] bool _callInitOnStart = false;

    [Header("Player Check")]
    [SerializeField] Vector3 _playerCheckCenter = Vector3.zero;
    [SerializeField] Vector3 _playerCheckExtends = Vector3.one;
    [SerializeField] LayerMask _playerCheckMask = 0;

    [Header("Haptic Feedback")]
    [SerializeField] float _pulsePhysicalDistance = 0.1f;
    [SerializeField] float _pulseIntensity = .75f;
    [SerializeField] float _pulseDuration = .05f;

    [Space]
    [Header("Tutorial")]
    [SerializeField] UnityEvent onHitEdge;

    [Space]
    public List<FillerDirections> fillerDirections = new();

    Vector3 _startPoint = Vector3.zero;
    float _prevLocalPos = 0f;
    float _pulseDelta = 0f;
    bool _atEnd = true;
    bool _hasHitOnce = false;

    List<InteractionPoint> _interactors = new List<InteractionPoint>();
    ThirdPersonMovement _player = null;
    PlayerMovableTile _tile = null;
    MovableShaderController _shader;
    SoundPlayer3D _soundPlayer;
    PlayerMovableSound _moveSound;
    ValueMover _valueMover;

    public PlayerMovableTile tile { get { return _tile; } internal set { _tile = value; } }

    public void Awake()
    {
        if (_playerCheckMask == 0)
            Debug.LogWarning(name + ": Player Check mask is set to 'None'");

        _moveSound = GetComponent<PlayerMovableSound>();
        if (_moveSound == null)
            Debug.LogError("Player Movable Sound Component not added");
    }

    private void Start()
    {
        _shader = GetComponentInChildren<MovableShaderController>();
        if (_shader == null)
            Debug.LogError(name + ": could not find Shader Controller in children");

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");

        _valueMover = FindObjectOfType<ValueMover>();
        if (_valueMover)
        {
            _valueMover.afterMoveEvent += MoveValues;
        }

        if (_callInitOnStart)
            Init(null);
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

    public void Init(PlayerMovableTile tile)
    {
        if (tile != null)
            _moveAmount = tile.moveAmount;

        _startPoint = transform.position;
    }

    private void Update()
    {
        CheckPlayer();
        if (_interactors.Count <= 0)
        {
            _moveSound.Off();
            return;
        }

        Vector3 controllerDelta = Vector3.zero;
        int count = 0;
        foreach (InteractionPoint interactor in _interactors)
        {
            if (interactor.isPressed)
            {
                controllerDelta += interactor.delta;
                ++count;
            }
        }

        if (count == 0)
        {
            _moveSound.Off();
            return;
        }

        controllerDelta /= count;

        // Get controller delta, apply along _moveAxis
        Vector3 projectedDelta = Vector3.Project(controllerDelta, _moveAmount);

        Vector3 projPoint = transform.position + projectedDelta - _startPoint;

        // Get Clamped Value
        float t = Vector3.Dot(projPoint, _moveAmount) / Vector3.Dot(_moveAmount, _moveAmount);
        t = Mathf.Clamp01(t);
        Vector3 finalLocalPos = t * _moveAmount;
        Vector3 finalPos = _startPoint + finalLocalPos;

        //MoveSound
        _moveSound.On();

        if (t == 1 && !_hasHitOnce)
        {
            _hasHitOnce = true;
            onHitEdge?.Invoke();
        }

        //End Sound
        bool newEnd = (t == 1f || t == 0f);
        if (newEnd && !_atEnd)
        {
            _soundPlayer.Play("user-movable-end", transform.position, SoundPlayer3D.Bank.Single);
        }
        _atEnd = newEnd;

        //Calculate Haptics
        float finalLocalMag = Vector3.Dot(finalLocalPos, _moveAmount.normalized);
        float finalDelta = finalLocalMag - _prevLocalPos;
        _prevLocalPos = finalLocalMag;
        _pulseDelta += finalDelta;

        if (_pulseDelta >= _pulsePhysicalDistance)
        {
            SendHaptic();
            while (_pulseDelta >= _pulsePhysicalDistance)
                _pulseDelta -= _pulsePhysicalDistance;
        }
        if (_pulseDelta <= -_pulsePhysicalDistance)
        {
            SendHaptic();
            while (_pulseDelta <= -_pulsePhysicalDistance)
                _pulseDelta += _pulsePhysicalDistance;
        }

        //Apply Position
        MoveTo(finalPos);
    }

    private void OnDestroy()
    {
        if (_valueMover)
        {
            _valueMover.afterMoveEvent -= MoveValues;
        }
    }

    private void MoveTo(Vector3 destination)
    {
        if (_player != null)
        {
            Vector3 delta = destination - transform.position;
            _player.Move(delta);
        }
        transform.position = destination;
    }

    private void SendHaptic()
    {
        foreach (InteractionPoint interactor in _interactors)
        {
            if (interactor.isPressed)
                interactor.SendHaptic(_pulseIntensity, _pulseDuration);
        }
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

    // ======= COLLISION EVENTS ========
    private void OnTriggerEnter(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;

        //interactionPoint.SendHaptic(_hapticIntensity, _hapticDuration);
        if (_interactors.Count == 0)
            _shader.Up();

        _interactors.Add(interactionPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;

        _interactors.Remove(interactionPoint);

        if (_interactors.Count == 0)
            _shader.Down();
    }

    private void MoveValues(float amount)
    {
        _startPoint += Vector3.up * amount;
    }
}
