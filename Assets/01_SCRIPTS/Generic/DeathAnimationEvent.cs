using UnityEngine;
using UnityEngine.Events;

public class DeathAnimationEvent : MonoBehaviour
{
    [SerializeField] UnityEvent _deathEvent;

    public void DeathEvent()
    {
        _deathEvent?.Invoke();
    }
}
