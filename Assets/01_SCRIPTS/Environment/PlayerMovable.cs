using System.Collections.Generic;
using UnityEngine;
using XephTools;

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
    [SerializeField] bool DEBUG_callAwake = false;

    [Header("Player Check")]
    [SerializeField] Vector3 _playerCheckCenter = Vector3.zero;
    [SerializeField] Vector3 _playerCheckExtends = Vector3.one;
    [SerializeField] LayerMask _playerCheckMask = 0;

    [Header("Haptic Feedback")]
    [SerializeField] float _pulsePhysicalDistance = 0.1f;
    [SerializeField] float _pulseIntensity = .75f;
    [SerializeField] float _pulseDuration = .05f;

    [Space]
    public List<FillerDirections> fillerDirections = new();

    Vector3 _startPoint = Vector3.zero;
    float _prevLocalPos = 0f;
    float _pulseDelta = 0f;

    List<InteractionPoint> _interactors = new List<InteractionPoint>();
    ThirdPersonMovement _player = null;
    PlayerMovableTile _tile = null;
    MovableShaderController _shader;

    public PlayerMovableTile tile { get { return _tile; } internal set { _tile = value; } }

    // TODO - REMOVE
    public void Awake()
    {
        if (_playerCheckMask == 0)
            Debug.LogWarning(name + ": Player Check mask is set to 'None'");
        if (DEBUG_callAwake)
            _startPoint = transform.position;
    }

    private void Start()
    {
        _shader = GetComponentInChildren<MovableShaderController>();
        if (_shader == null)
            Debug.LogError(name + ": could not find Shader Controller in children");
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

        foreach(TileBase filler in _tile.FillerTiles)
        {
            Vector3 pos = level.transform.TransformPoint(filler.gridCoord);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos, Vector3.one * 0.5f);
        }
    }

    public void Init(PlayerMovableTile tile)
    {
        _moveAmount = tile.moveAmount;
        _startPoint = transform.position;
    }

    private void Update()
    {
        CheckPlayer();
        VRDebug.Monitor(5, (_player != null));
        if (_interactors.Count <= 0)
            return;

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
            return;

        controllerDelta /= count;

        // Get controller delta, apply along _moveAxis
        Vector3 projectedDelta = Vector3.Project(controllerDelta, _moveAmount);

        Vector3 projPoint = transform.position + projectedDelta - _startPoint;

        // Get Clamped Value
        float t = Vector3.Dot(projPoint, _moveAmount) / Vector3.Dot(_moveAmount, _moveAmount);
        t = Mathf.Clamp01(t);
        Vector3 finalLocalPos = t * _moveAmount;
        Vector3 finalPos = _startPoint + finalLocalPos;

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

    private void MoveTo(Vector3 destination)
    {
        if (_player != null)
        {
            Vector3 delta = destination - transform.position;
            VRDebug.Monitor(6, delta);
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
                _player = null;
            return;
        }

        if (_player != null)
            return;

        foreach (Collider collider in collisions)
        {
            _player = collider.GetComponent<ThirdPersonMovement>();
            if (_player != null)
                return;
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
}
