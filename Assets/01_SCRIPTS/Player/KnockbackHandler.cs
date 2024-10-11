using System.Collections;
using UnityEngine;
using XephTools;

public class KnockbackHandler : MonoBehaviour
{
    public enum AgentType { Player, MeleeEnemy, RangedEnemy}

    [SerializeField] AgentType _agentType = AgentType.Player;

    ThirdPersonMovement _playerMovement;
    EnemyMovementController _meleeMovement;
    EnemySimpleGravity _rangedMovement;

    Vector3 _velocity = Vector3.zero;

    bool _isBusy = false;
    OverTime.ModuleReference<Vector3Lerp> _overtimeRef = null;

    private void Awake()
    {
        _playerMovement = GetComponent<ThirdPersonMovement>();
        _meleeMovement = GetComponent<EnemyMovementController>();
        _rangedMovement = GetComponent<EnemySimpleGravity>();

        if (_agentType == AgentType.MeleeEnemy && !_meleeMovement)
            Debug.LogWarning("Knockback handler set to Melee Enemy, but cannot find EnemyMovementController component");

        if (_agentType == AgentType.Player && !_playerMovement)
            Debug.LogWarning("Knockback handler set to Player, but cannot find ThirdPersonMovement component");

        if (_agentType == AgentType.RangedEnemy && !_rangedMovement)
            Debug.LogWarning("Knockback handler set to Ranged Enemy, but cannot find EnemySimpleGravity component");
    }

    public void StartKnockback(Vector3 initVelocity, float duration)
    {
        if (_isBusy)
        {
            _overtimeRef.Get().End();
        }

        _isBusy = true;

        switch (_agentType)
        {
        case AgentType.Player:
            StartPlayer(initVelocity, duration);
            break;
        case AgentType.RangedEnemy:
            StartRangedEnemy(initVelocity, duration);
            break;
        case AgentType.MeleeEnemy:
            StartMeleeEnemy(initVelocity, duration);
            break;
        }

    }

    private void StartPlayer(Vector3 initVelocity, float duration)
    {
        Vector3Lerp velocityLerp = new(initVelocity, Vector3.zero, duration, (Vector3 v) => { _playerMovement.Move(v * Time.deltaTime); });
        velocityLerp.OnComplete(() => _isBusy = false);
        _overtimeRef = OverTime.Add(velocityLerp);
    }

    private void StartMeleeEnemy(Vector3 initVelocity, float duration)
    {
        Vector3Lerp velocityLerp = new(initVelocity, Vector3.zero, duration, (Vector3 v) => { _meleeMovement.Move(v * Time.deltaTime); });
        velocityLerp.OnComplete(() => _isBusy = false);
        _overtimeRef = OverTime.Add(velocityLerp);
    }

    private void StartRangedEnemy(Vector3 initVelocity, float duration)
    {
        Vector3Lerp velocityLerp = new(initVelocity, Vector3.zero, duration, (Vector3 v) => { _rangedMovement.Move(v * Time.deltaTime); });
        velocityLerp.OnComplete(() => _isBusy = false);
        _overtimeRef = OverTime.Add(velocityLerp);
    }
}
