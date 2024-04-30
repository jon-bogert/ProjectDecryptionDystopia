using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] float _rangeRadius = 5f;
    [SerializeField] UnityEvent _onAttack;

    ThirdPersonMovement _player;

    enum EnemyState { Idle, Patrol, Attack }
    EnemyState _currentState = EnemyState.Idle;
    EnemyState _nextState = EnemyState.Idle;

    private void Start()
    {
        _player = FindObjectOfType<ThirdPersonMovement>();
        if (!_player)
        {
            Debug.LogError("Could not find player in scene");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _rangeRadius);
    }

    private void Update()
    {
        _currentState = _nextState;
        switch (_currentState)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
            case EnemyState.Patrol:
                PatrolUpdate();
                break;
            case EnemyState.Attack:
                AttackUpdate();
                break;
            default:
                Debug.LogWarning("EnemyState not implemented");
                break;
        }
    }

    private void IdleUpdate()
    {
        if (CheckRadius())
        {
            Debug.Log("Attacking");
            _nextState = EnemyState.Attack;
            _onAttack?.Invoke();
        }
    }

    private void PatrolUpdate()
    {
        if (CheckRadius())
        {
            _nextState = EnemyState.Attack;
            _onAttack?.Invoke();
        }
    }

    private void AttackUpdate()
    {

    }

    private bool CheckRadius()
    {
        float offsetSqr = (transform.position - _player.transform.position).sqrMagnitude;
        return (offsetSqr < Mathf.Pow(_rangeRadius, 2));
    }
}
