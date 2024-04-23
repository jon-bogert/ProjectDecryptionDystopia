using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    public enum DoorState
    {
        Closed,
        Open,
        Opening
    }

    [Header("Parameters")]
    [SerializeField] float _timeTotal = 0.3f;

    [Header("References")]
    [SerializeField] Transform _doorLeft;
    [SerializeField] Transform _doorRight;

    Vector3 _positionStart = Vector3.zero;

    float _timer = 0;
    float _timeTotalInv = 1f;
    DoorState _doorState = DoorState.Closed;

    public DoorState doorState { get { return _doorState; } }

    private void Awake()
    {
        _timeTotalInv = 1f / _timeTotal;
        _positionStart = _doorLeft.localPosition;
    }

    private void Update()
    {
        if (doorState != DoorState.Opening)
            return;

        if(_timer > _timeTotal)
        {
            _doorState = DoorState.Open;
            return;
        }

        Vector3 leftOffset = Vector3.Lerp(Vector3.zero, Vector3.right, _timer * _timeTotalInv);
        Vector3 rightOffset = Vector3.Lerp(Vector3.zero, Vector3.left, _timer * _timeTotalInv);

        _doorLeft.localPosition = _positionStart + leftOffset;
        _doorRight.localPosition = _positionStart + rightOffset;

        _timer += Time.deltaTime;
    }

    public void OpenDoor()
    {
        if (_doorState != DoorState.Closed)
            return;

        _timer = 0f;
        _doorState = DoorState.Opening;
    }
}
