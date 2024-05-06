using UnityEngine;
using UnityEngine.Events;

public class EditorButton : MonoBehaviour
{
    [SerializeField] LayerMask _mask = 0;
    [SerializeField] UnityEvent _onPress;

    bool _isPressed = false;
    GameObject _presser = null;

    private void Awake()
    {
        if (_mask == 0)
            Debug.LogError(name + " Editor button layer mask set to none");

        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isPressed)
            return;

        int layerMask = 1 << other.gameObject.layer;

        if ((layerMask & _mask.value) == 0)
            return;

        _presser = other.gameObject;
        _isPressed = true;
        Press();
        _onPress?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isPressed)
            return;

        if (other.gameObject != _presser)
            return;

        _presser = null;
        _isPressed = false;
        UnPress();
    }

    void Press()
    {
        GetComponent<MeshRenderer>().material.color = Color.green;
    }

    void UnPress()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
