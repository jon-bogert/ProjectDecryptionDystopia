using TMPro;
using UnityEngine;
using XephTools;

public class TextFader : MonoBehaviour, ISequenceable
{
    [SerializeField] float _time = .15f;
    [SerializeField] bool _startClear = true;
    [Space]
    [Header("Optional:")]
    [SerializeField] TMP_Text _field = null;

    Color _color = Color.white;
    Color _colorClear = Color.clear;

    private void Start()
    {
        if (_field == null)
            _field = GetComponent<TMP_Text>();

        _color = _field.color;
        _colorClear = new Color(_color.r, _color.g, _color.b, 0f);

        if (_startClear)
            _field.color = _colorClear;
    }

    public void Complete(bool isOpen) { } // Do Nothing

    public float BeginGetLength(bool isOpening)
    {
        return _time;
    }

    public void SequenceValue(float val)
    {
        _field.color = Color.Lerp(_colorClear, _color, val);
    }
}
