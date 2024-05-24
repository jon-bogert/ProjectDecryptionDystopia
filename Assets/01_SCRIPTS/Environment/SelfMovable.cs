using UnityEngine;
using XephTools;



public class SelfMovable : MonoBehaviour
{
    public enum State { Start, End, ToStart, ToEnd };

    [SerializeField] Vector3 _moveAmount = Vector3.zero;
    [SerializeField] bool _isOscillating = false;
    [SerializeField] float _moveTime = 1f;
    [SerializeField] float _easeAmount = 0.1f;

    State _state = State.Start;
    Vector3 _startPosition = Vector3.zero;

    OverTime.ModuleReference<Vector3CubicLerp> _lerpRef;

    private void Start()
    {
        _startPosition = transform.position;
    }

    public void Toggle()
    {
        if (_isOscillating)
        {
            OscToggle();
            return;
        }

        float progress = 0f;
        if (_state == State.ToEnd || _state == State.ToStart)
        {
            progress = _lerpRef.Get().Progress;
            _lerpRef.Get().End();
        }

        if (_state == State.End || _state == State.ToEnd)
        {
            _state = State.ToStart;
            Vector3 startPos = (_state == State.End) ? _startPosition + _moveAmount : transform.position;
            Vector3CubicLerp lerp = new(startPos, _startPosition + _moveAmount,
                _startPosition + _moveAmount * (1f - _easeAmount),
                _startPosition + _moveAmount * _easeAmount,
                _moveTime - (progress * _moveTime),
                (val) => { transform.position = val; }
                );
            lerp.OnComplete(() => _state = State.Start);
            _lerpRef = OverTime.Add(lerp);
            return;
        }

        _state = State.ToEnd;
        Vector3 startPos2 = (_state == State.Start) ? _startPosition : transform.position;
        Vector3CubicLerp lerp2 = new(startPos2, _startPosition + _moveAmount,
            _startPosition + _moveAmount * _easeAmount,
            _startPosition + _moveAmount * (1 - _easeAmount),
            _moveTime - (progress * _moveTime),
            (val) => { transform.position = val; }
            );
        lerp2.OnComplete(() => _state = State.End);
        _lerpRef = OverTime.Add(lerp2);
    }

    private void OscToggle()
    {

    }
}
