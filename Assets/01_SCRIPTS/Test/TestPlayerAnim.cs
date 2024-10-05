using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAnim : MonoBehaviour
{
    [SerializeField] InputAction _attackInput;
    [SerializeField] Animator _animator;

    // Start is called before the first frame update
    void Awake()
    {
        _attackInput.Enable();
        _attackInput.performed += DoAttack;
    }

    private void DoAttack(InputAction.CallbackContext callbackContext)
    {
        _animator.SetTrigger("Attack");
    }
}
