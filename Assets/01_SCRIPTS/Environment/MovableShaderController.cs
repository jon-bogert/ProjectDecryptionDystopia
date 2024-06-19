using UnityEngine;
using XephTools;

[RequireComponent(typeof(MeshRenderer))]
public class MovableShaderController : MonoBehaviour
{
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;

    enum State { Down, Up, GoingDown, GoingUp };

    float _appliedValue = 0f;
    float _value = 0f;
    State _state = State.Down;

    [SerializeField] float _lerpTime = 1f;

    Material _material;
    Material _lineMaterial;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (_value == _appliedValue)
            return;

        _material.SetFloat("_Transition", _value);
        _lineMaterial.SetFloat("_Alpha", _value);
        _appliedValue = _value;
    }

    public void SetLineMaterial(Material lineMat)
    {
        _lineMaterial = lineMat;
        _lineMaterial.SetFloat("_Alpha", 0f);
    }

    public void Up()
    {
        if (_state == State.Up || _state == State.GoingUp)
            return;

        if (_state == State.GoingDown)
        {
            float progress = _lerpRef.Get().Progress;
            _lerpRef.Get().End();
            _state = State.GoingUp;

            OverTime.LerpModule lerp = new(progress, 1f,
                _lerpTime - (progress * _lerpTime),
                (float v) => { _value = v; });

            lerp.OnComplete(() => _state = State.Up);
            _lerpRef = OverTime.Add(lerp);
            return;
        }

        _state = State.GoingUp;

        OverTime.LerpModule lerp2 = new(0f, 1f,
            _lerpTime,
            (float v) => { _value = v; });

        lerp2.OnComplete(() => _state = State.Up);
        _lerpRef = OverTime.Add(lerp2);
    }

    public void Down()
    {
        if (_state == State.Down || _state == State.GoingDown)
            return;

        if (_state == State.GoingUp)
        {
            float progress = _lerpRef.Get().Progress;
            _lerpRef.Get().End();
            _state = State.GoingDown;

            OverTime.LerpModule lerp = new(progress, 0f,
                progress * _lerpTime,
                (float v) => { _value = v; });

            lerp.OnComplete(() => _state = State.Down);
            _lerpRef = OverTime.Add(lerp);
            return;
        }

        _state = State.GoingDown;

        OverTime.LerpModule lerp2 = new(1f, 0f,
            _lerpTime,
            (float v) => { _value = v; });

        lerp2.OnComplete(() => _state = State.Down);
        _lerpRef = OverTime.Add(lerp2);
    }
}
