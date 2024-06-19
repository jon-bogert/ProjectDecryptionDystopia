using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecenterOrigin : MonoBehaviour
{
    [SerializeField] InputActionReference _holdInput;
    [SerializeField] InputActionReference _keyboardInput;

    XROrigin _origin;

    private void Awake()
    {
        _holdInput.action.performed += Recenter;
        _keyboardInput.action.performed += Recenter;

        _origin = GetComponent<XROrigin>();
    }

    private void Start()
    {
        transform.position = SceneLoader.inst.originOffset;
        transform.rotation = SceneLoader.inst.originRotation;
    }

    private void OnDestroy()
    {
        _holdInput.action.performed -= Recenter;
        _keyboardInput.action.performed -= Recenter;
    }

    private void Recenter(InputAction.CallbackContext ctx)
    {
        //_origin.MoveCameraToWorldLocation(Vector3.zero);
        Vector3 posXZ = new Vector3(
            Camera.main.transform.position.x,
            0f,
            Camera.main.transform.position.z);
        transform.position = transform.position - posXZ;
        _origin.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);

        SceneLoader.inst.originOffset = transform.position;
        SceneLoader.inst.originRotation = transform.rotation;
    }
}
