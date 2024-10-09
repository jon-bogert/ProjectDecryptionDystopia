using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using XephTools;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] float _damage = 0.25f;
    [SerializeField] float _knockbackSpeed = 10f;
    [SerializeField] float _knockbackDuration = 0.3f;
    [SerializeField] float _comboWindowTime = 0.5f;
    [Header("Movement")]
    [SerializeField] float _smallDashSpeed = 5f;
    [SerializeField] float _smallDashDuration = 0.2f;
    [SerializeField] float _largeDashSpeed = 10f;
    [SerializeField] float _largeDashDuration = 0.2f;
    [Header("References")]
    [SerializeField] Hurtbox _hurtbox;
    [SerializeField] Animator _animator;
    [Header("Inputs")]
    [SerializeField] InputActionReference _attackInput;

    SoundPlayer3D _soundPlayer;
    ThirdPersonMovement _thirdPersonMovement;
    KnockbackHandler _playerKnockback;
    bool _isAttacking = false;
    float _isAttackingCheckDelay = 0.5f;
    float _isAttackingCheckTimer = 0f;
    bool _doAttack = false;

    private void Awake()
    {
        _thirdPersonMovement = GetComponent<ThirdPersonMovement>();
        if (_thirdPersonMovement == null)
            Debug.LogWarning("ThirdPersonMovement Component not found");

        _playerKnockback = GetComponent<KnockbackHandler>();
        if (_playerKnockback == null)
            Debug.LogWarning("Player's KockbackHandler Component not found");

    }

    private void Start()
    {
        _hurtbox.onHurt += OnHurt;
        if (_animator == null)
            Debug.LogWarning(name + ": Arm Animator not assigned in inspector");

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    private void Update()
    {
        if (_isAttacking && _isAttackingCheckTimer <= 0f)
        {
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Walk-Run"))
            {
                _isAttacking = false;
                _thirdPersonMovement.isImmobile = false;
            }
        }
        else if (_isAttacking)
        {
            _isAttackingCheckTimer -= Time.deltaTime;
        }

        if (_attackInput.action.WasPressedThisFrame() && !_hurtbox.isHurting && !_doAttack)
        {
            _doAttack = true;
            _animator.SetBool("DoAttack", _doAttack);
            _isAttacking = true;
            _thirdPersonMovement.isImmobile = true;
            _isAttackingCheckTimer = _isAttackingCheckDelay;
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
            button.Interact();
            return;
        }
        Health health = collision.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError(collision.name + " has no health component");
            return;
        }
        KnockbackHandler knockback = collision.GetComponent<KnockbackHandler>();
        if (knockback != null)
        {
            knockback.StartKnockback(transform.forward * _knockbackSpeed, _knockbackDuration);
        }
        else
        {
            Debug.LogWarning(collision.name + " has no knockback handler component");
        }

        _soundPlayer.Play("melee-hit-player", transform.position, SoundPlayer3D.Bank.Single);
        health.TakeDamage(_damage);
    }

    public void ResetAttackBool()
    {
        _doAttack = false;
        _animator.SetBool("DoAttack", _doAttack);
    }

    public void DoSmallDash()
    {
        _playerKnockback.StartKnockback(transform.forward * _smallDashSpeed, _smallDashDuration);
    }

    public void DoLargeDash()
    {
        _playerKnockback.StartKnockback(transform.forward * _largeDashSpeed, _largeDashDuration);
    }
}
