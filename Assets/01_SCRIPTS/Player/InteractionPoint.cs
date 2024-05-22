using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class InteractionPoint : MonoBehaviour
{
    [SerializeField] InputActionReference _interactInput;

     HapticImpulsePlayer _controller;

    private void Start()
    {
        _controller = GetComponentInParent<HapticImpulsePlayer>();
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
