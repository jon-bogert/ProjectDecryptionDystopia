using UnityEngine;
using XephTools;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] Vector3 _detectSize = Vector3.one;
    [SerializeField] Vector3 _detectCenter = Vector3.zero;
    [SerializeField] LayerMask _playerMask = 0;
    [SerializeField] float _attackCooldown = 1f;
    [SerializeField] float _stunTime = 5f;
    [Header("References")]
    [SerializeField] Animator _armAnimator;

    bool _canAttack = true;
    bool _isAttacking = false;

    TimeIt _stunTimer = new TimeIt();
    bool _isStunned = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_detectCenter, _detectSize);
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
    }

    private void Update()
    {
        if (!_isAttacking
            || !_canAttack
            || _isStunned)
            return;

        Collider[] colliders = Physics.OverlapBox(transform.position + _detectCenter, _detectSize * 0.5f, transform.rotation, _playerMask);
        if (colliders.Length <= 0)
            return;

        _armAnimator.SetTrigger("DoAttack");
        _canAttack = false;

        TimeIt timer = new();
        timer.OnComplete(() => { _canAttack = true; }).SetDuration(_attackCooldown).Start();
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
            _stunTimer.OnComplete(() => { _isStunned = false; }).SetDuration(_stunTime).Start();
        }
        else
        {
            _stunTimer.SetDuration(_stunTime);
        }
    }
}
