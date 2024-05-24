using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Transformation;
using XephTools;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _moveSpeed = 3f;
    [SerializeField] float _gravityAmount = 10f;
    [SerializeField] float _jumpAmount = 5f;

    [Header("Settings")]
    [SerializeField] bool _doCameraAdjust = true;

    [Header("Inputs")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _jumpInput;

    float _verticalVelocity = 0f;
    bool _isGrounded = false;

    CharacterController _charController;
    Transform _camera;

    private void Awake()
    {
        _charController = GetComponent<CharacterController>();
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        //_isGrounded = _groundChecker.CheckGround();
        _isGrounded = _charController.isGrounded;
        VRDebug.Monitor(1, _isGrounded ? "t" : "f");

        Vector2 moveAxis = _moveInput.action.ReadValue<Vector2>();
        Vector3 moveFinal = new Vector3(
            moveAxis.x,
            0f,
            moveAxis.y
        );

        AdjustByCamera(ref moveFinal);

        FaceMovement(moveFinal);

        float jumpAmt = ProcJump();
        VRDebug.Monitor(3, jumpAmt);

        if (_isGrounded && jumpAmt == 0f)
            _verticalVelocity = 0f;
        else if (jumpAmt > 0)
            _verticalVelocity = jumpAmt;

        _verticalVelocity -= _gravityAmount * Time.deltaTime;

        moveFinal *= _moveSpeed;
        moveFinal.y = _verticalVelocity;

        _charController.Move( moveFinal * Time.deltaTime );
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
        VRDebug.Monitor(2, _jumpInput.action.ReadValue<float>());
        if (!_isGrounded)
            return 0f;

        bool jumpPressed = _jumpInput.action.ReadValue<float>() > float.Epsilon;
        if (!jumpPressed)
            return 0f;

        Debug.Log("Jump Pressed");
        return _jumpAmount;
    }

    public void Move(Vector3 amount)
    {
        _charController.Move(amount);
    }
}
