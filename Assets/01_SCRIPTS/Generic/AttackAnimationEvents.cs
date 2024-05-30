using UnityEngine;
using UnityEngine.Events;

public class AttackAnimationEvents : MonoBehaviour
{
    [SerializeField] UnityEvent damageStart;
    [SerializeField] UnityEvent damageEnd;
    [SerializeField] UnityEvent rangedAttack;

    public void DamageStart()
    {
        damageStart?.Invoke();
    }

    public void DamageEnd()
    {
        damageEnd?.Invoke();
    }

    public void RangedAttack()
    {
        rangedAttack?.Invoke();
    }
}
