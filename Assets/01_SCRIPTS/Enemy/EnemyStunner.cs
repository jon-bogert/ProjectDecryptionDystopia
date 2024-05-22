using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStunner : MonoBehaviour
{
    [SerializeField] UnityEvent _stunEvent;

    List<InteractionPoint> _interactors = new List<InteractionPoint>();

    private void Update()
    {
        foreach(InteractionPoint interactionPoint in _interactors)
        {
            if (interactionPoint.isPressed)
            {
                _stunEvent?.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;
        
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
