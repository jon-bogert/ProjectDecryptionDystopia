using UnityEngine;
using XephTools;

public class DissolveInOutSingle : MonoBehaviour
{
    public enum State { In, Out }
    [SerializeField] Material _material;
    [SerializeField] State _state = State.Out;
    [SerializeField] float _transitionTime = 0.1f;

    float _currentState = 0f;

    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;

    private void Awake()
    {
        if ( _material == null )
        {
            _material = GetComponent<Renderer>().material;
        }

        _currentState = (_state == State.In) ? 1 : 0;
        _material.SetFloat("_Dissolve", _currentState);
    }

    public void DissolveIn()
    {
        if (_state == State.In)
            return;

        _state = State.In;
        if (_lerpRef != null && !_lerpRef.IsExpired())
        {
            _lerpRef.Get().End();
        }

        OverTime.LerpModule lerp = new(0f, 1f, _transitionTime, (float t) =>
        {
            _currentState = t;
            _material.SetFloat("_Dissolve", _currentState);
        });

        _lerpRef = OverTime.Add(lerp);
    }

    public void DissolveOut()
    {
        if (_state == State.Out)
            return;

        _state = State.Out;
        if (_lerpRef != null && !_lerpRef.IsExpired())
        {
            _lerpRef.Get().End();
        }

        OverTime.LerpModule lerp = new(1f, 0f, _transitionTime, (float t) =>
        {
            _currentState = t;
            _material.SetFloat("_Dissolve", _currentState);
        });

        _lerpRef = OverTime.Add(lerp);
    }
}
