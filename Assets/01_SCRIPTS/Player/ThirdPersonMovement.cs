using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _gravityAmount = 10f;
    [SerializeField] float _jumpAmount = 5f;
    [SerializeField] float _deadZone = 0.25f;
    [SerializeField] LayerMask _movableMask = 0;
    [SerializeField] float _movableMinimum = 0.1f;

    [Header("Fall Check")]
    [SerializeField] float _fallLength = 10f;
    [SerializeField] LayerMask _fallMask = 0;

    [Header("References")]
    [SerializeField] Animator _legAnimator;

    [Header("Settings")]
    [SerializeField] bool _doCameraAdjust = true;

    [Header("Inputs")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _jumpInput;

    float _verticalVelocity = 0f;
    bool _isGrounded = false;
    bool _isJumping = false;

    CharacterController _charController;
    Transform _camera;
    StepsAudioPlayer _stepAudio;
    SoundPlayer3D _soundPlayer;

    Vector3 _moveDelta = Vector3.zero;
    Vector3 _extMoveDelta = Vector3.zero;

    private void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _camera = Camera.main.transform;

        if (_legAnimator == null)
            Debug.LogWarning(name + ": Leg Animator not assigned in inspector");

        if (_fallMask == 0)
            Debug.LogWarning(name + ": Fall Mask is set to None");

        if (_movableMask == 0)
            Debug.LogWarning(name + ": Movable Mask is set to None");
    }

    private void Start()
    {
        _stepAudio = GetComponentInChildren<StepsAudioPlayer>();
        if (_stepAudio == null)
            Debug.LogError("ThirdPersonMovement:Could not find StepsAudioPlayer");

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Could not find Sound Player in Scene");
    }

    private void Update()
    {
        //Just Landed
        //if(_charController.isGrounded && !_isGrounded)
        //{
        //    _stepAudio.Play();
        //}
        _isGrounded = _charController.isGrounded;

        Vector2 moveAxis = _moveInput.action.ReadValue<Vector2>();
        if (moveAxis.sqrMagnitude < _deadZone)
            moveAxis = Vector2.zero;

        Vector3 moveFinal = new Vector3(
            moveAxis.x,
            0f,
            moveAxis.y
        );

        if (moveFinal == Vector3.zero)
        {
            _legAnimator.SetFloat("WalkBlend", 0);
        }
        else
        {
            _legAnimator.SetFloat("WalkBlend", moveAxis.magnitude);
        }

        AdjustByCamera(ref moveFinal);

        FaceMovement(moveFinal);

        float jumpAmt = ProcJump();

        if (_isGrounded && jumpAmt == 0f)
        {
            _verticalVelocity = 0f;
            if (_isJumping)
            {
                _isJumping = false;
                _legAnimator.SetBool("IsJumping", false);
            }
        }
        else if (jumpAmt > 0)
        {
            _verticalVelocity = jumpAmt;
            _isJumping = true;
            _legAnimator.SetBool("IsJumping", true);
        }

        _verticalVelocity -= _gravityAmount * Time.deltaTime;

        moveFinal *= _moveSpeed;

        //Ledge Check
        if (_isGrounded)
        {
            Vector3 newPos = transform.position + moveFinal * Time.deltaTime;
            bool isHit = Physics.Raycast(newPos, Vector3.down, _fallLength, _fallMask);
            if (!isHit)
            {
                moveFinal = Vector3.zero;
            }
        }

        moveFinal.y = _verticalVelocity;

        _moveDelta += moveFinal * Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (_charController == null)
            return;

        Vector3 finalMove = _moveDelta + _extMoveDelta;
        if (_extMoveDelta.y > 0f)
        {
            bool isHit = Physics.Raycast(transform.position, Vector3.down, _movableMinimum, _movableMask);
            float finalY = (isHit) ? _extMoveDelta.y : _moveDelta.y;
            finalMove = new Vector3(finalMove.x, finalY, finalMove.z);
        }  

        _charController.Move(finalMove);
        _moveDelta = Vector3.zero;
        _extMoveDelta = Vector3.zero;
    }

    private void AdjustByCamera(ref Vector3 moveFinal)
    {
        if (!_doCameraAdjust)
            return;

        Vector3 camFrwd = _camera.transform.forward;
        camFrwd.y = 0f;
        camFrwd.Normalize();

        Vector3 camRight = _camera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        moveFinal = camRight * moveFinal.x + camFrwd * moveFinal.z;
    }

    private void FaceMovement(Vector3 moveVector)
    {
        if (moveVector.sqrMagnitude == float.Epsilon)
            return;

        moveVector.Normalize();
        Vector3 lookPoint = transform.position + moveVector;
        transform.LookAt( lookPoint );
    }

    private float ProcJump()
    {
        if (!_isGrounded)
            return 0f;

        bool jumpPressed = _jumpInput.action.ReadValue<float>() > float.Epsilon;
        if (!jumpPressed)
            return 0f;

        _soundPlayer?.Play("jump", transform.position, SoundPlayer3D.Bank.Single);
        return _jumpAmount;
    }

    public void Move(Vector3 amount)
    {
        _extMoveDelta += amount;
    }
}
