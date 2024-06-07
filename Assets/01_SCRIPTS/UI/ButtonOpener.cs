using UnityEngine;
using XephTools;

public class ButtonOpener : MonoBehaviour, ISequenceable
{
    [SerializeField] float _time = .25f;
    [SerializeField] bool _startClosed = true;

    Vector3 _scale = Vector3.one;

    ButtonBounds _bounds;
    SoundPlayerUI _sounds;

    private void Start()
    {
        _scale = transform.localScale;
        _bounds = GetComponent<ButtonBounds>();
        if (_bounds == null)
        {
            Debug.LogError("No button Button Bounds component found");
        }

        if (_startClosed)
        {
            transform.localScale = new Vector3(0f, _scale.y, _scale.z);
            _bounds.isActive = false;
        }

        _sounds = FindObjectOfType<SoundPlayerUI>();
        if (_sounds == null)
        {
            Debug.LogWarning("UI Sounds Player wasn't found in current scene");
        }
    }

    public void Complete(bool isOpen)
    {
        if (isOpen)
            _bounds.isActive = true;
    }

    public float BeginGetLength(bool isOpening)
    {
        if (!isOpening)
            _bounds.isActive = false;

        _sounds?.Play("button-open");

        return _time;
    }

    public void SequenceValue(float val)
    {
        transform.localScale = new Vector3(_scale.x * val, _scale.y, _scale.z);
    }
}
