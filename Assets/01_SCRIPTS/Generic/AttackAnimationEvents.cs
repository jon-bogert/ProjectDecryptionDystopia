using UnityEngine;
using UnityEngine.Events;

public class AttackAnimationEvents : MonoBehaviour
{
    [SerializeField] UnityEvent damageStart;
    [SerializeField] UnityEvent damageEnd;

    public void DamageStart()
    {
        damageStart?.Invoke();
    }

    public void DamageEnd()
    {
        damageEnd?.Invoke();
    }
}
