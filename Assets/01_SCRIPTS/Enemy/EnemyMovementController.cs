using TMPro;
using UnityEngine;
using XephTools;

[RequireComponent (typeof(EnemySeek))]
[RequireComponent (typeof(CharacterController))]
public class EnemyMovementController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _gravityAmount = 10f;
    [SerializeField] float _stunTime = 5f;
    [SerializeField] bool _disableSeek = false;
    [SerializeField] bool _facePlayerAlways = false;
    [Header("References")]
    [SerializeField] Animator _animator;
    [Space]
    [Header("Tutorial")]
    [SerializeField] bool _contributeToTutCount = false;
    [Header("Debug")]
    [SerializeField] bool _showDebug = false;

    ThirdPersonMovement _player;
    EnemySeek _seek;
    CharacterController _charController;

    Vector3 _moveDelta = Vector3.zero;
    Vector3 _extMoveDelta = Vector3.zero;

    TimeIt _stunTimer = new();

    bool _isGrounded = false;
    float _verticalVelocity = 0f;
    bool _isAttacking = false;
    bool _isStunned = false;
    bool _hasStunnedOnce = false;

    private void Awake()
    {
        _seek = GetComponent<EnemySeek>();
        _charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _player = FindObjectOfType<ThirdPersonMovement>();
        if (_player == null)
            Debug.LogError(name + " could not find Player in scene");
    }

    private void Update()
    {
        if (_facePlayerAlways)
        {
            transform.LookAt(new Vector3(
                _player.transform.position.x,
                transform.position.y,
                _player.transform.position.z));
        }

        if (_disableSeek || !_isAttacking || _isStunned)
        {
            _isGrounded = _charController.isGrounded;
            if (_isGrounded)
                _verticalVelocity = 0f;

            _verticalVelocity -= _gravityAmount * Time.deltaTime;
            _moveDelta += Vector3.up * _verticalVelocity * Time.deltaTime;

            _animator.SetFloat("WalkBlend", 0f);

            return;
        }

        Vector3 playerPos = _player.transform.position;
        if (!_seek.Seek(playerPos, out Vector2 direction2D))
        {
            Vector3 direction3D = new Vector3(direction2D.x, 0f, direction2D.y);
            InternalMove(direction3D);
            _animator.SetFloat("WalkBlend", direction3D.magnitude);
        }
        else
        {
            _animator.SetFloat("WalkBlend", 0f);
            Vector3 direction3D = new Vector3(direction2D.x, 0f, direction2D.y);
            FaceMovement(direction3D);
            if (_showDebug) Debug.Log("Arrived");
        }
    }

    private void LateUpdate()
    {
        if (_charController == null)
            return;

        Vector3 finalMove = _moveDelta + _extMoveDelta;
        if (_extMoveDelta.y > 0f)
        {
            _charController.enabled = false;
            finalMove = new Vector3(finalMove.x, _extMoveDelta.y, finalMove.z);
            transform.position += finalMove;
            _charController.enabled = true;
        }
        else
        {
            _charController.Move(finalMove);
        }

        _moveDelta = Vector3.zero;
        _extMoveDelta = Vector3.zero;
    }

    public void Stun()
    {
        _isStunned = true;

        if (_stunTimer.isExpired)
        {
            _animator.speed = 0f;
            _stunTimer.OnComplete(() =>
            {
                _isStunned = false;
                _animator.speed = 1f;
                if (!_hasStunnedOnce)
                {
                    _hasStunnedOnce = true;
                    if (_contributeToTutCount)
                        FindObjectOfType<TutorialManager>().IncreaseEnemyCounter();
                }
            }).SetDuration(_stunTime).Start();
        }
        else
        {
            _stunTimer.SetDuration(_stunTime);
        }
    }
    
    public void OnAttack()
    {
        _isAttacking = true;
    }

    private void InternalMove(Vector3 direction)
    {
        _isGrounded = _charController.isGrounded;
        FaceMovement(direction);

        if (_isGrounded)
            _verticalVelocity = 0f;

        _verticalVelocity -= _gravityAmount * Time.deltaTime;
        Vector3 moveFinal = direction * _moveSpeed;
        moveFinal.y = _verticalVelocity;
        _moveDelta = moveFinal * Time.deltaTime;
        //_charController.Move(moveFinal * Time.deltaTime);
    }

    private void FaceMovement(Vector3 moveVector)
    {
        if (moveVector.sqrMagnitude == float.Epsilon)
            return;

        moveVector.Normalize();
        Vector3 lookPoint = transform.position + moveVector;
        transform.LookAt(lookPoint);
    }

    public void Move(Vector3 amount)
    {
        _extMoveDelta += amount;
    }
}
