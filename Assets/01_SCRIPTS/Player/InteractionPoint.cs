using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionPoint : MonoBehaviour
{
    [SerializeField] InputActionReference _interactInput;
    public bool isPressed { get { return _interactInput.action.IsPressed(); } }
}
