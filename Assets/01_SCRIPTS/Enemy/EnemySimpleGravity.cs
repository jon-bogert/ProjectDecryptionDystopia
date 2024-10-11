using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class EnemySimpleGravity : MonoBehaviour
{
    [SerializeField] float _gravity = 10f;
    [SerializeField] LayerMask _fallMask = 0;

    float _velocity = 0f;

    CharacterController _charController;

    Vector3 _externalVelocityBuffer = Vector3.zero;

    private void Awake()
    {
        if (_fallMask == 0)
        {
            Debug.LogWarning(name + ": Fall mask not set");
        }

        _charController = GetComponent<CharacterController>();
    }

    private void LateUpdate()
    {
        if (!_charController.enabled)
            return;

        _velocity += _gravity * Time.deltaTime;

        if (_charController.isGrounded)
        {
            _velocity = 0f;
        }

        Vector3 finalMove = _externalVelocityBuffer + (Vector3.down * _gravity * Time.deltaTime);
        _externalVelocityBuffer = Vector3.zero;
        //Ledge Check
        Vector3 newPos = transform.position + finalMove;
        bool isHit = Physics.Raycast(newPos, Vector3.down, 1, _fallMask);
        if (!isHit)
        {
            finalMove.x = finalMove.z = 0f;
        }

        _charController.Move(finalMove);
    }

    public void Move(Vector3 amount)
    {
        _externalVelocityBuffer += amount;
    }
}
