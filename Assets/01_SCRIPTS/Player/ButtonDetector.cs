using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonDetector : MonoBehaviour
{
    [SerializeField] Vector3 _dimensions = Vector3.one;
    [SerializeField] LayerMask _buttonMask = 0;
    [Header("References")]
    [SerializeField] Transform _detectTransform;

    Button _activeButton = null;
    PlayerAttack _attack;

    private void Awake()
    {
        if (_buttonMask == 0)
        {
            Debug.LogWarning("Button Detector layer mask not set");
        }
    }

    private void Start()
    {
        _attack = GetComponent<PlayerAttack>();
        if (!_attack)
            Debug.LogError("Could not find Player Attack Component");

        _attack.pressButton += OnPressButton;
    }

    private void Update()
    {
        Collider[] results = Physics.OverlapBox(_detectTransform.position, _dimensions, _detectTransform.rotation, _buttonMask);
        if (results.Length > 1)
        {
            Debug.LogWarning("Button Detector result greater than one");
        }
        if (results.Length == 0)
        {
            if ( _activeButton != null )
            {
                _attack.doInteract = false;
                _activeButton.ButtonLeave();
                _activeButton = null;
            }
            return;
        }

        _activeButton = results[0].GetComponent<Button>();
        if (_activeButton == null )
        {
            Debug.LogError("Button Detector collision did not have Button component");
            return;
        }
        _attack.doInteract = true;
        _activeButton.ButtonApproach();
    }

    private void OnDestroy()
    {
        if (_attack)
        {
            _attack.pressButton -= OnPressButton;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_detectTransform == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(_detectTransform.position, _dimensions);
    }

    private void OnPressButton()
    {
        if (!_activeButton)
        {
            Debug.LogError("No active button to press");
            return;
        }

        _activeButton.Interact();
    }
}
