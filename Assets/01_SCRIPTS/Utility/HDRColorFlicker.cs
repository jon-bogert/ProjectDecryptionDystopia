using UnityEngine;

public class HDRColorFlicker : MonoBehaviour
{
    Color _originalValue;
    [SerializeField] string _propertyName;
    [SerializeField] float _upper = 1.1f;
    [SerializeField] float _lower = 0.8f;
    [SerializeField] float _rate = 0.1f;
    [SerializeField] float _minDifference = 0f;

    float _timer;
    Color _prevValue;
    float _prevMultiple = 1f;
    Color _targetValue;

    Material _material;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    private void Start()
    {
        _originalValue = _material.GetColor(_propertyName);
        _prevValue = _originalValue;
        _targetValue = NewValue();
    }

    private void Update()
    {
        if (_timer >= _rate)
        {
            _timer = 0;
            _prevValue = _targetValue;
            _targetValue = NewValue();
            _material.SetColor(_propertyName, _prevValue);
            return;
        }

        _material.SetColor(_propertyName, Color.Lerp(_prevValue, _targetValue, _timer / _rate));

        _timer += Time.deltaTime;
    }

    Color NewValue()
    {
        float multiple = 0f;
        do
        {
            multiple = Random.Range(_upper, _lower);
        } while (Mathf.Abs(_prevMultiple - multiple) < _minDifference);

        _prevMultiple = multiple;
        return _originalValue * multiple;
    }
}