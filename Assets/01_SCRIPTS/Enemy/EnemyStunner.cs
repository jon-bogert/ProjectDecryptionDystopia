using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStunner : MonoBehaviour
{
    [SerializeField] UnityEvent _stunEvent;
    [Range(0f, 1f)]
    [SerializeField] float _hapticIntensity = 0.7f;
    [SerializeField] float _hapticDuration = 0.01f;

    List<InteractionPoint> _interactors = new List<InteractionPoint>();

    private void Update()
    {
        foreach(InteractionPoint interactionPoint in _interactors)
        {
            if (interactionPoint.isPressed)
            {
                interactionPoint.SendHaptic(_hapticIntensity, _hapticDuration);
                _stunEvent?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;
        
        interactionPoint.SendHaptic(_hapticIntensity, _hapticDuration);
        _interactors.Add(interactionPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;

        _interactors.Remove(interactionPoint);
    }


}
