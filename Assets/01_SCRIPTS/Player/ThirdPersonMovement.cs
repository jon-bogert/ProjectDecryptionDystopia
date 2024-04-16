using UnityEngine;
using UnityEngine.InputSystem;
using XephTools;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _moveSpeed = 0.01f;
    [SerializeField] float _gravityAmount = 0.1f;
    [SerializeField] float _jumpAmount = 0.1f;

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
}
