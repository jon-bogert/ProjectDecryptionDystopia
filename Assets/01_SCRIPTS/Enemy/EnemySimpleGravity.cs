using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class EnemySimpleGravity : MonoBehaviour
{
    [SerializeField] float _gravity = 10f;

    float _velocity = 0f;

    CharacterController _charController;

    private void Awake()
    {
        _charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!_charController.enabled)
            return;

        if (_charController.isGrounded)
        {
            _velocity = 0f;
        }

        _velocity += _gravity * Time.deltaTime;
        _charController.Move(Vector3.down * _gravity * Time.deltaTime);
    }
}
