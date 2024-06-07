using TMPro;
using UnityEngine;
using XephTools;

public class TextSequence : MonoBehaviour, ISequenceable
{
    [SerializeField] float _time = 1f;
    [SerializeField] bool _startEmpty = true;
    [Space]
    [Header("Optional:")]
    [SerializeField] string _contents = "";
    [SerializeField] string _soundTag = "";
    [SerializeField] TMP_Text _field = null;

    SoundPlayerUI _sounds;

    private void Start()
    {
        if (_field == null)
            _field = GetComponent<TMP_Text>();
        if (_contents == "")
            _contents = _field.text;

        if (_startEmpty)
            _field.text = "";

        _sounds = FindObjectOfType<SoundPlayerUI>();
        if (_sounds == null)
        {
            Debug.LogWarning("UI Sounds Player wasn't found in current scene");
        }
    }

    public void Complete(bool isOpen) { } // Do Nothing

    public float BeginGetLength(bool isOpening)
    {
        if (_soundTag != "")
            _sounds?.Play(_soundTag);
        return _time;
    }

    public void SequenceValue(float val)
    {
        int charsToDisplay = (int)(_contents.Length * val);
        _field.text = _contents.Substring(0, charsToDisplay);
    }
}
