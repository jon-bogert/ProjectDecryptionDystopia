using UnityEngine;
using XephTools;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] Vector3 _detectSize = Vector3.one;
    [SerializeField] LayerMask _playerMask = 0;
    [SerializeField] float _attackDelay = 0.1f;
    [SerializeField] float _attackCooldown = 1f;
    [SerializeField] float _stunTime = 5f;
    [SerializeField] float _damage = .25f;
    [SerializeField] float _knockbackSpeed = 5f;
    [SerializeField] float _knockbackDuration = 0.25f;
    [Header("References")]
    [SerializeField] Transform _detectTransform;
    [SerializeField] Animator _animator;

    EnemyMovementController _movement;
    Hurtbox _hurtbox;
    SoundPlayer3D _soundPlayer;

    bool _canAttack = true;
    bool _isAttacking = false;
    bool _animDoAttack = false; // Mirrors DoAttack in animator
    float _isAttackingCheckTimer = 0f;
    float _isAttackingCheckDelay = 0.5f;

    TimeIt _stunTimer = new TimeIt();
    bool _isStunned = false;

    TimeIt _delayTimer = new();

    private void OnDrawGizmosSelected()
    {
        if (_detectTransform == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_detectTransform.position, _detectSize);
    }

    private void Awake()
    {
        _movement = GetComponent<EnemyMovementController>();
    }

    private void Start()
    {
        if (_playerMask == 0)
        {
            Debug.LogWarning(name + ": Player mask set to None");
        }
        if (_animator == null)
        {
            Debug.LogError(name + ": Arm Animator not assigned in inspector");
        }
        if (_detectTransform == null)
        {
            Debug.LogError(name + ": Detect Transform not assigned in inspector");
        }

        _hurtbox = GetComponentInChildren<Hurtbox>();
        if (_hurtbox == null)
            Debug.LogError(name + ": Could not fine hurtbox in children");

        _hurtbox.onHurt += OnHurt;

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
                _movement.isImmobile = false;
            }
        }
        else if (_isAttacking)
        {
            if (_movement.isStunned)
                _isAttackingCheckTimer = 0f;
            _isAttackingCheckTimer -= Time.deltaTime;
        }

        if (!_isAttacking
            || _animDoAttack
            || !_canAttack
            || _isStunned)
            return;

        Collider[] colliders = Physics.OverlapBox(_detectTransform.position, _detectSize * 0.5f, transform.rotation, _playerMask);
        if (colliders.Length <= 0) // No Player detected
        {
            if (!_delayTimer.isExpired)
                _delayTimer.Abort();

            return;
        }
        if (!_delayTimer.isExpired)
        {
            return;
        }

        _delayTimer.OnComplete(() =>
        {
            _animDoAttack = true;
            _animator.SetBool("DoAttack", _animDoAttack);
            _movement.isImmobile = true;
            _isAttackingCheckTimer = _isAttackingCheckDelay;
            _canAttack = false;

            TimeIt timer = new();
            timer.OnComplete(() => { _canAttack = true; }).SetDuration(_attackCooldown).Start();
        }).SetDuration(_attackDelay).Start();
    }

    private void OnDestroy()
    {
        if (_hurtbox != null)
            _hurtbox.onHurt -= OnHurt;
    }

    public void StartAttacking()
    {
        _isAttacking = true;
    }

    public void ResetAttackBool()
    {
        _animDoAttack = false;
        _animator.SetBool("DoAttack", _animDoAttack);
    }

    public void Stun()
    {
        _isStunned = true;

        if (_stunTimer.isExpired)
        {
            _animator.speed = 0f;
            _stunTimer.OnComplete(() => { _isStunned = false; _animator.speed = 1f; }).SetDuration(_stunTime).Start();
        }
        else
        {
            _stunTimer.SetDuration(_stunTime);
        }
    }

    private void OnHurt(Collider collision)
    {
        Health health = collision.GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError(collision.name + " has no health component");
            return;
        }
        KnockbackHandler knockback = collision.GetComponent<KnockbackHandler>();
        if (knockback == null)
        {
            Debug.LogError(collision.name + " has no knockback component");
            return;
        }
        _soundPlayer.Play("melee-hit-enemy", transform.position, SoundPlayer3D.Bank.Single);
        health.TakeDamage(_damage);
        knockback.StartKnockback((collision.transform.position - transform.position).normalized * _knockbackSpeed, _knockbackDuration);
    }

    public void OnDeath()
    {
        _isAttacking = false;
    }
}
