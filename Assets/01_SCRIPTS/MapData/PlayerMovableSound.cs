using Unity.VisualScripting;
using UnityEngine;
using XephTools;

[RequireComponent(typeof(AudioSource))]
public class PlayerMovableSound : MonoBehaviour
{
    [SerializeField] float _volume = 0.75f;
    [SerializeField] float _fadeTime = 0.15f; 

    AudioSource _source;
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef = null;

    enum State { Off, On, GoingUp, GoingDown }
    State _state = State.Off;

    public bool isOn { get { return _state == State.On || _state == State.GoingUp;} }

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.volume = 0f;
        _source.loop = true;
    }

    public void On()
    {
        if (_state == State.On || _state == State.GoingUp)
            return;

        if (_state == State.Off)
        {
            _state = State.GoingUp;
            _source.Play();
            OverTime.LerpModule lerp0 = new(0f, _volume, _fadeTime, VolumeSetter);
            lerp0.OnComplete(() => _state = State.On);
            _lerpRef = OverTime.Add(lerp0);
            return;
        }

        // _state == State.GoingDown
        _state = State.GoingUp;
        float t = 1f - _lerpRef.Get().Progress;
        float start = t * _volume;
        _lerpRef.Get().End(_lerpRef.Get().Progress);

        OverTime.LerpModule lerp1 = new(start, _volume, _fadeTime - (t * _fadeTime), VolumeSetter);
        lerp1.OnComplete(() => _state = State.On);
        _lerpRef = OverTime.Add(lerp1);
    }

    public void Off()
    {
        if (_state == State.Off || _state == State.GoingDown)
            return;

        if (_state == State.On)
        {
            _state = State.GoingDown;
            OverTime.LerpModule lerp0 = new(_volume, 0, _fadeTime, VolumeSetter);
            lerp0.OnComplete(() => {_source.Stop(); _state = State.Off;});
            _lerpRef = OverTime.Add(lerp0);
            return;
        }

        // _state == State.GoingUp
        _state = State.GoingDown;
        float t = _lerpRef.Get().Progress;
        float start = t * _volume;
        _lerpRef.Get().End(_lerpRef.Get().Progress);

        OverTime.LerpModule lerp1 = new(start, 0f, _fadeTime - ((1f - t) * _fadeTime), VolumeSetter);
        lerp1.OnComplete(() => { _source.Stop(); _state = State.Off; });
        _lerpRef = OverTime.Add(lerp1);
    }

    private void VolumeSetter(float vol)
    {
        _source.volume = vol;
    }
}
