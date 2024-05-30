using UnityEngine;
using XephTools;

[RequireComponent (typeof(EnemySeek))]
[RequireComponent (typeof(CharacterController))]
public class EnemyMovementController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _gravityAmount = 10f;
    [SerializeField] float _stunTime = 5f;
    [Header("References")]
    [SerializeField] Animator _legAnimator;
    [Space]
    [Header("Debug")]
    [SerializeField] bool _showDebug = false;

    ThirdPersonMovement _player;
    EnemySeek _seek;
    CharacterController _charController;

    TimeIt _stunTimer = new();

    bool _isGrounded = false;
    float _verticalVelocity = 0f;
    bool _isAttacking = false;
    bool _isStunned = false;

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
        if (!_isAttacking || _isStunned)
        {
            _isGrounded = _charController.isGrounded;
            if (_isGrounded)
                _verticalVelocity = 0f;

            _verticalVelocity -= _gravityAmount * Time.deltaTime;
            _charController.Move(Vector3.up * _verticalVelocity * Time.deltaTime);

            _legAnimator.SetFloat("WalkBlend", 0f);

            return;
        }

        Vector3 playerPos = _player.transform.position;
        if (!_seek.Seek(playerPos, out Vector2 direction2D))
        {
            Vector3 direction3D = new Vector3(direction2D.x, 0f, direction2D.y);
            Move(direction3D);
            _legAnimator.SetFloat("WalkBlend", direction3D.magnitude);
        }
        else
        {
            _legAnimator.SetFloat("WalkBlend", 0f);
            Vector3 direction3D = new Vector3(direction2D.x, 0f, direction2D.y);
            FaceMovement(direction3D);
            if (_showDebug) Debug.Log("Arrived");
        }
    }

    public void Stun()
    {
        _isStunned = true;

        if (_stunTimer.isExpired)
        {
            _legAnimator.speed = 0f;
            _stunTimer.OnComplete(() => { _isStunned = false; _legAnimator.speed = 1f; }).SetDuration(_stunTime).Start();
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

    private void Move(Vector3 direction)
    {
        _isGrounded = _charController.isGrounded;
        FaceMovement(direction);

        if (_isGrounded)
            _verticalVelocity = 0f;

        _verticalVelocity -= _gravityAmount * Time.deltaTime;
        Vector3 moveFinal = direction * _moveSpeed;
        moveFinal.y = _verticalVelocity;
        _charController.Move(moveFinal * Time.deltaTime);
    }

    private void FaceMovement(Vector3 moveVector)
    {
        if (moveVector.sqrMagnitude == float.Epsilon)
            return;

        moveVector.Normalize();
        Vector3 lookPoint = transform.position + moveVector;
        transform.LookAt(lookPoint);
    }
}
