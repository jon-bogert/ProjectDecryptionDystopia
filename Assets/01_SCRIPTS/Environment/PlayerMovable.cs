using System.Collections.Generic;
using UnityEngine;
using XephTools;

public class PlayerMovable : MonoBehaviour
{
    [SerializeField] Vector3 _moveAmount = Vector3.zero;
    [SerializeField] bool DEBUG_callAwake = false;

    Vector3 _startPoint = Vector3.zero;

    List<InteractionPoint> _interactors = new List<InteractionPoint>();

    // TODO - REMOVE
    public void Awake()
    {
        if (DEBUG_callAwake)
            Init(_moveAmount);
    }

    public void Init(Vector3 moveAmount)
    {
        _moveAmount = moveAmount;

        _startPoint = transform.position;
    }

    private void Update()
    {
        if (_interactors.Count <= 0)
            return;

        Vector3 controllerDelta = Vector3.zero;
        int count = 0;
        foreach (InteractionPoint interactor in _interactors)
        {
            if (interactor.isPressed)
            {
                controllerDelta += interactor.delta;
                ++count;
            }
        }

        if (count == 0)
            return;

        controllerDelta /= count;

        VRDebug.Monitor(5, controllerDelta);
        // Get controller delta, apply along _moveAxis
        Vector3 projectedDelta = Vector3.Project(controllerDelta, _moveAmount);
        VRDebug.Monitor(6, projectedDelta);

        Vector3 projPoint = transform.position + projectedDelta - _startPoint;

        // Get Clamped Value
        float t = Vector3.Dot(projPoint, _moveAmount) / Vector3.Dot(_moveAmount, _moveAmount);
        t = Mathf.Clamp01(t);
        Vector3 finalPos = _startPoint + (t * _moveAmount);

        //Apply
        transform.position = finalPos;
        //transform.position += projectedDelta;
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;

        //interactionPoint.SendHaptic(_hapticIntensity, _hapticDuration);
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
