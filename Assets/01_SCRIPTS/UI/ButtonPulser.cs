using UnityEngine;

[RequireComponent (typeof(ButtonBounds))]
[RequireComponent (typeof(MeshRenderer))]
public class ButtonPulser : MonoBehaviour
{
    [SerializeField] float _pulseAmount = 0.5f;
    [SerializeField] float _pulseTime = 0.1f;
    [SerializeField] float _totalTime = 1f;
    [SerializeField] int _numPulses = 2;

    Material _material;
    ButtonBounds _bounds;
    float _time = 0;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _bounds = GetComponent<ButtonBounds>();
    }

    private void Update()
    {
        float value = 0f;
        if (_bounds.isActive && _time < _pulseTime * _numPulses)
        {
            float t = (_time % (_pulseTime * 2 / (_numPulses * 2f)));
            int i = (int)(_time / (_pulseTime * 2 / (_numPulses * 2f)));
            if (i % 2 == 0) // Up
            {
                value = Mathf.Lerp(0f, 1f, t / (_pulseTime * 0.5f));
            }
            else // Down
            {
                value = Mathf.Lerp(1f, 0f, t / (_pulseTime * 0.5f));
            }
        }

        _material.SetFloat("_Selected", value * _pulseAmount);

        _time += Time.deltaTime;
        while (_time >= _totalTime)
        {
            _time -= _totalTime;
        }
    }
}
