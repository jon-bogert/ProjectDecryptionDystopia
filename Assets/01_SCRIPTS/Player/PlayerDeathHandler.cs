using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] Animator _legAnimator;
    [SerializeField] Animator _armAnimator;

    public void Invoke()
    {
        _legAnimator.speed = 1f;
        _armAnimator.speed = 1f;

        _legAnimator.SetTrigger("OnDeath");
        _armAnimator.SetTrigger("OnDeath");
    }
}
