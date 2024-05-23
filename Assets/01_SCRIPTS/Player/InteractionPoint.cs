using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class InteractionPoint : MonoBehaviour
{
    [SerializeField] InputActionReference _interactInput;

    HapticImpulsePlayer _controller;
    Vector3 _prevPosition;
    Vector3 _delta;

    public Vector3 delta { get { return _delta; } }

    private void Start()
    {
        _controller = GetComponentInParent<HapticImpulsePlayer>();
    }

    private void Update()
    {
        _delta = transform.position - _prevPosition;
        _prevPosition = transform.position;
    }

    public bool isPressed { get { return _interactInput.action.IsPressed(); } }
    public void SendHaptic(float intensity, float duration)
    {
        if (duration <= 0f)
            return;
        if (intensity <= 0f)
            return;
        if (intensity > 1f)
            intensity = 1f;

        _controller.SendHapticImpulse(intensity, duration);
    }
}
