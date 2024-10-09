using UnityEngine;
using XephTools;

public class KnockbackHandler : MonoBehaviour
{
    [SerializeField] bool _isEnemy = false;

    ThirdPersonMovement _playerMovement;
    EnemyMovementController _enemyMovement;

    Vector3 _velocity = Vector3.zero;

    bool _isBusy = false;
    OverTime.ModuleReference<Vector3Lerp> _overtimeRef = null;

    private void Awake()
    {
        _playerMovement = GetComponent<ThirdPersonMovement>();
        _enemyMovement = GetComponent<EnemyMovementController>();

        if (_isEnemy && !_enemyMovement)
            Debug.LogWarning("Knockback handler set to Enemy, but cannot find EnemyMovementController component");

        if (!_isEnemy && !_playerMovement)
            Debug.LogWarning("Knockback handler set to Player, but cannot find ThirdPersonMovement component");
    }

    public void StartKnockback(Vector3 initVelocity, float duration)
    {
        if (_isBusy)
        {
            _overtimeRef.Get().End();
        }

        _isBusy = true;

        if (_isEnemy)
        {
            StartEnemy(initVelocity, duration);
            return;
        }

        StartPlayer(initVelocity, duration);
    }

    private void StartPlayer(Vector3 initVelocity, float duration)
    {
        Vector3Lerp velocityLerp = new(initVelocity, Vector3.zero, duration, (Vector3 v) => { _playerMovement.Move(v * Time.deltaTime); });
        velocityLerp.OnComplete(() => _isBusy = false);
        _overtimeRef = OverTime.Add(velocityLerp);
    }

    private void StartEnemy(Vector3 initVelocity, float duration)
    {
        Vector3Lerp velocityLerp = new(initVelocity, Vector3.zero, duration, (Vector3 v) => { _enemyMovement.Move(v * Time.deltaTime); });
        velocityLerp.OnComplete(() => _isBusy = false);
        _overtimeRef = OverTime.Add(velocityLerp);
    }
}
