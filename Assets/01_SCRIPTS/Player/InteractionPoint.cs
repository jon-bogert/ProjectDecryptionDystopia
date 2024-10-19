using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class InteractionPoint : MonoBehaviour
{
    [SerializeField] InputActionReference _interactInput;

    HapticImpulsePlayer _controller;
    Vector3 _prevPosition;
    Vector3 _delta;

    PlayerMovable _currentMovable = null;

    public Vector3 delta { get { return _delta; } }

    public Action onPress;
    public Action onRelease;

    public PlayerMovable currentMovable
    {
        get { return _currentMovable; }
        set
        {
            if (_currentMovable != null)
                return;

            _currentMovable = value;
        }
    }

    private void Start()
    {
        _controller = GetComponentInParent<HapticImpulsePlayer>();
    }

    private void Update()
    {
        _delta = transform.position - _prevPosition;
        _prevPosition = transform.position;

        if (_interactInput.action.WasPressedThisFrame())
        {
            onPress?.Invoke();
        }
        if (_interactInput.action.WasReleasedThisFrame())
        {
            onRelease?.Invoke();
            if (_currentMovable != null)
            {
                _currentMovable.ReleaseHold(this);
                _currentMovable = null;
            }
        }
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
