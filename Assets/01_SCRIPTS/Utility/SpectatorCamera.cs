using UnityEngine;

/// <summary>
/// Smooth follow motion for spectator camera
/// Will destroy itself (and rest of GO) in Android release build
/// </summary>
public class SpectatorCamera : MonoBehaviour
{
    [SerializeField] Transform _cameraTransform;
    [Range(0f,1f)]
    [SerializeField] float _positionSmooth = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] float _rotationSmooth = 0.5f;

    private void Awake()
    {
#if UNITY_ANDROID
        if (!Debug.isDebugBuild)
        {
            Destroy(gameObject);
            return;
        }
#endif

        if (_cameraTransform == null)
        {
            Debug.LogError("SpectatorCamera -> No Camera Transform assigned in inspector");
            return;
        }
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        ResetPosition();
    }

    private void Update()
    {
        if (_cameraTransform == null)
            return;

        if ((transform.position - _cameraTransform.position).sqrMagnitude > 25)
        {
            ResetPosition();
            return;
        }

        transform.position = Vector3.Lerp(_cameraTransform.position, transform.position, _positionSmooth);
        transform.rotation = Quaternion.Slerp(_cameraTransform.rotation, transform.rotation, _rotationSmooth);
    }

    private void ResetPosition()
    {
        if (_cameraTransform == null)
            return;

        transform.position = _cameraTransform.position;
        transform.rotation = _cameraTransform.rotation;
    }
}
