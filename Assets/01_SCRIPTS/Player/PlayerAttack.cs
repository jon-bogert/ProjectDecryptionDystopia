using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float _damage = 0.25f;
    [Header("References")]
    [SerializeField] Hurtbox _hurtbox;
    [Header("Inputs")]
    [SerializeField] InputActionReference _attackInput;

    private void Start()
    {
        _hurtbox.onHurt += OnHurt;
    }

    private void Update()
    {
        if (_attackInput.action.WasPressedThisFrame() && !_hurtbox.isHurting)
        {
            // TODO - Start Animation instead
            //        and have animation call HurtStart and HurtEnd

            _hurtbox.HurtStart(); // NOTE: Temp Hurtbox has length as non-zero
        }
    }

    private void OnDestroy()
    {
        _hurtbox.onHurt -= OnHurt;
    }

    private void OnHurt(Collider collision)
    {
        Health health = collision.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError(collision.name + " has no health component");
            return;
        }

        health.TakeDamage(_damage);
    }
}
