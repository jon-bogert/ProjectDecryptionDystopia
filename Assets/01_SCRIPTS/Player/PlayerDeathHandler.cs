using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] Animator _legAnimator;
    [SerializeField] Animator _armAnimator;

    public void Invoke()
    {
        _legAnimator.SetTrigger("OnDeath");
        _armAnimator.SetTrigger("OnDeath");
    }
}
