using UnityEngine;
using XephTools;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] Vector3 _detectSize = Vector3.one;
    [SerializeField] LayerMask _playerMask = 0;
    [SerializeField] float _attackCooldown = 1f;
    [SerializeField] float _stunTime = 5f;
    [SerializeField] float _damage = .25f;
    [Header("References")]
    [SerializeField] Transform _detectTransform;
    [SerializeField] Animator _armAnimator;

    Hurtbox _hurtbox;

    bool _canAttack = true;
    bool _isAttacking = false;

    TimeIt _stunTimer = new TimeIt();
    bool _isStunned = false;

    private void OnDrawGizmosSelected()
    {
        if (_detectTransform == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_detectTransform.position, _detectSize);
    }

    private void Start()
    {
        if (_playerMask == 0)
        {
            Debug.LogWarning(name + ": Player mask set to None");
        }
        if (_armAnimator == null)
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
    }

    private void Update()
    {
        if (!_isAttacking
            || !_canAttack
            || _isStunned)
            return;

        Collider[] colliders = Physics.OverlapBox(_detectTransform.position, _detectSize * 0.5f, transform.rotation, _playerMask);
        if (colliders.Length <= 0)
            return;

        _armAnimator.SetTrigger("DoAttack");
        _canAttack = false;

        TimeIt timer = new();
        timer.OnComplete(() => { _canAttack = true; }).SetDuration(_attackCooldown).Start();
    }

    private void OnDestroy()
    {
        _hurtbox.onHurt -= OnHurt;
    }

    public void StartAttacking()
    {
        _isAttacking = true;
    }

    public void Stun()
    {
        _isStunned = true;

        if (_stunTimer.isExpired)
        {
            _armAnimator.speed = 0f;
            _stunTimer.OnComplete(() => { _isStunned = false; _armAnimator.speed = 1f; }).SetDuration(_stunTime).Start();
        }
        else
        {
            _stunTimer.SetDuration(_stunTime);
        }
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

        health.TakeDamage(_damage);
    }
}
