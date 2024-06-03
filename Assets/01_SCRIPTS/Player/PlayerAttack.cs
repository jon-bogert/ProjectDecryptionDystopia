using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float _damage = 0.25f;
    [Header("References")]
    [SerializeField] Hurtbox _hurtbox;
    [SerializeField] Animator _armAnimator;
    [Header("Inputs")]
    [SerializeField] InputActionReference _attackInput;

    SoundPlayer3D _soundPlayer;

    private void Start()
    {
        _hurtbox.onHurt += OnHurt;
        if (_armAnimator == null)
            Debug.LogWarning(name + ": Arm Animator not assigned in inspector");

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    private void Update()
    {
        if (_attackInput.action.WasPressedThisFrame() && !_hurtbox.isHurting)
        {
            _armAnimator.SetTrigger("DoAttack");
        }
    }

    private void OnDestroy()
    {
        _hurtbox.onHurt -= OnHurt;
    }

    private void OnHurt(Collider collision)
    {
        Button button = collision.GetComponent<Button>();
        if (button)
        {
            button.Insteract();
            return;
        }
        Health health = collision.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError(collision.name + " has no health component");
            return;
        }

        _soundPlayer.Play("melee-hit-player", transform.position, SoundPlayer3D.Bank.Single);
        health.TakeDamage(_damage);
    }
}
