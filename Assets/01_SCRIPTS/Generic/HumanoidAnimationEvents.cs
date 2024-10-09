using UnityEngine;
using UnityEngine.Events;

public class HumanoidAnimationEvents : MonoBehaviour
{
    //Attack
    [SerializeField] UnityEvent animationBegin;
    [SerializeField] UnityEvent animationEnd;
    [SerializeField] UnityEvent animationReturn;
    [SerializeField] UnityEvent resetAttackBool;

    [Space]

    [SerializeField] UnityEvent damageStart;
    [SerializeField] UnityEvent damageEnd;
    [SerializeField] UnityEvent jumpForwardSmall;
    [SerializeField] UnityEvent jumpForwardLarge;
    [SerializeField] UnityEvent rangedAttack;


    public void ResetAttackBool()
    {
        resetAttackBool.Invoke();
    }

    public void AnimationBegin()
    {
        animationBegin?.Invoke();
    }

    public void AnimationEnd()
    {
        animationEnd?.Invoke();
    }

    public void OnReturn()
    {
        animationReturn?.Invoke();
    }

    public void DamageStart()
    {
        damageStart?.Invoke();
    }

    public void DamageEnd()
    {
        damageEnd?.Invoke();
    }

    public void JumpForwardSmall()
    {
        jumpForwardSmall?.Invoke();
    }

    public void JumpForwardLarge()
    {
        jumpForwardLarge?.Invoke();
    }

    public void RangedAttack()
    {
        rangedAttack?.Invoke();
    }

}
