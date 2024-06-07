using UnityEngine;
using UnityEngine.XR;

public class BeginButtonFollow : MonoBehaviour
{
    [SerializeField] float _buttonOffset = 2f;
    [SerializeField] float _smoothTime = 0.9f;
    [SerializeField] float _triggerDistance = 10f;
    Transform _camera;
    Vector3 _velocity;

    private void Start()
    {
        _camera = Camera.main.transform;
    }

    private void Update()
    {
        InputDevice headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        bool userPresent = false;
        if (headDevice.isValid)
        {
            bool presenceFeatureSupported = headDevice.TryGetFeatureValue(CommonUsages.userPresence, out userPresent);
        }
        if (!userPresent)
            return;

        Vector3 pos = transform.position;
        Vector3 newPos = new Vector3(pos.x, _camera.position.y - _buttonOffset, pos.z);

        if (Mathf.Abs(newPos.y - pos.y) < _triggerDistance && _velocity.sqrMagnitude < 0.1f)
            return;

        transform.position = Vector3.SmoothDamp(pos, newPos, ref _velocity, 0.9f);
    }

}
