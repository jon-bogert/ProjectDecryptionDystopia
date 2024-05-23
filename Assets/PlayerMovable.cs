using UnityEngine;

public class PlayerMovable : MonoBehaviour
{
    Vector3 _moveAmount = Vector3.zero;
    Vector3 _startPoint = Vector3.zero;

    bool _isMoving = false;

    public void Init(Vector3 moveAmount)
    {
        _moveAmount = moveAmount;

        _startPoint = transform.position;
    }

    private void Update()
    {
        if (!_isMoving)
            return;

        // Get controller delta, apply along _moveAxis
        Vector3 controllerDelta = Vector3.zero; // TODO - Get Actual
        Vector3 projectedDelta = Vector3.Project(controllerDelta, _moveAmount);

        // Get Clamped Value
        float t = Vector3.Dot(projectedDelta, _moveAmount) / Vector3.Dot(_moveAmount, _moveAmount);
        t = Mathf.Clamp01(t);
        Vector3 finalPos = _startPoint + (t * _moveAmount);

        // Apply
    }
}
