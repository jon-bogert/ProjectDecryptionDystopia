using UnityEngine;
using UnityEngine.Events;
using XephTools;

public class MenuBoundsAnimator : MonoBehaviour
{
    public enum State { Closed, Open }
    [SerializeField] State _state = State.Closed;
    [SerializeField] float _transitionTime = 0.5f;
    [SerializeField] float _pauseTime = 0.25f;
    [SerializeField] float _weightValue = 0.1f;
    [SerializeField] float _minVal = 0.001f;
    [SerializeField] bool _closeOnLookAway = false;
    [SerializeField] string _soundTag = "";
    [Space]
    public UnityEvent openStarted;
    public UnityEvent openEnded;
    public UnityEvent closedStarted;
    public UnityEvent closedEnded;
    public UnityEvent closedEndedB;

    bool _isTransitioning = false;
    Vector3 _fullScale = Vector3.one;
    Vector3 _stage0 = Vector3.one;
    Vector3 _stage1 = Vector3.one;
    Vector3 _stage2 = Vector3.one;

    Vector3 _stage0WA = Vector3.one;
    Vector3 _stage0WB = Vector3.one;
    Vector3 _stage1WA = Vector3.one;
    Vector3 _stage1WB = Vector3.one;
    Vector3 _stage2WA = Vector3.one;
    Vector3 _stage2WB = Vector3.one;

    SoundPlayerUI _sounds;

    public State state { get { return _state; } }

    private void Start()
    {
        InitValues();

        _sounds = FindObjectOfType<SoundPlayerUI>();
        if (_sounds == null)
        {
            Debug.LogWarning("UI Sounds Player wasn't found in current scene");
        }

        if (_state == State.Closed)
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }
    }

    private void Update()
    {
        if (!_closeOnLookAway || state != State.Open)
            return;

        CheckClose();
    }

    private void InitValues()
    {
        _fullScale = transform.localScale;
        _stage0 = new Vector3(_minVal, _minVal, _minVal);
        _stage1 = new Vector3(_fullScale.x, _minVal, _minVal);
        _stage2 = new Vector3(_fullScale.x, _fullScale.y, _minVal);

        _stage0WA = _stage0 + Vector3.right * _weightValue;
        _stage0WB = _stage1 - Vector3.right * _weightValue;

        _stage1WA = _stage1 + Vector3.up * _weightValue;
        _stage1WB = _stage2 - Vector3.up * _weightValue;

        _stage2WA = _stage2 + Vector3.forward * _weightValue;
        _stage2WB = _fullScale - Vector3.forward * _weightValue;
    }

    public void Open()
    {
        if (_isTransitioning || _state == State.Open)
            return;

        gameObject.SetActive(true);
        _isTransitioning = true;
        openStarted?.Invoke();

        if (_soundTag != "")
            _sounds?.Play(_soundTag);

        //Vector3CubicLerp lerpA = new(_stage0, _stage1, _stage0WA, _stage0WB, _transitionTime, ScaleSetter);
        //Vector3CubicLerp lerpB = new(_stage1, _stage2, _stage1WA, _stage1WB, _transitionTime, ScaleSetter);
        //Vector3CubicLerp lerpC = new(_stage2, _fullScale, _stage2WA, _stage2WB, _transitionTime, ScaleSetter);
        Vector3Lerp lerpA = new(_stage0, _stage1, _transitionTime, ScaleSetter);
        Vector3Lerp lerpB = new(_stage1, _stage2, _transitionTime, ScaleSetter);
        Vector3Lerp lerpC = new(_stage2, _fullScale, _transitionTime, ScaleSetter);

        TimeIt timerA = new();
        timerA.SetDuration(_pauseTime).OnComplete(() => OverTime.Add(lerpB));
        TimeIt timerB = new();
        timerB.SetDuration(_pauseTime).OnComplete(() => OverTime.Add(lerpC));

        lerpA.OnComplete(() => timerA.Start());
        lerpB.OnComplete(() => timerB.Start());
        lerpC.OnComplete(EndOpen);

        OverTime.Add(lerpA);
    }

    private void EndOpen()
    {
        _state = State.Open;
        _isTransitioning = false;
        openEnded?.Invoke();
    }

    public void Close(bool useEndB)
    {
        if (_isTransitioning || _state == State.Closed)
            return;

        _isTransitioning = true;
        closedStarted?.Invoke();
        if (_soundTag != "")
            _sounds?.Play(_soundTag);

        //Vector3CubicLerp lerpA = new( _fullScale, _stage2, _stage2WB, _stage2WA, _transitionTime, ScaleSetter );
        //Vector3CubicLerp lerpB = new( _stage2, _stage1, _stage1WB, _stage1WA, _transitionTime, ScaleSetter );
        //Vector3CubicLerp lerpC = new( _stage1, _stage0, _stage0WB, _stage0WA, _transitionTime, ScaleSetter );
        Vector3Lerp lerpA = new(_fullScale, _stage2,  _transitionTime, ScaleSetter);
        Vector3Lerp lerpB = new(_stage2, _stage1,  _transitionTime, ScaleSetter);
        Vector3Lerp lerpC = new(_stage1, _stage0, _transitionTime, ScaleSetter);

        TimeIt timerA = new();
        timerA.SetDuration(_pauseTime).OnComplete(() => OverTime.Add(lerpB));
        TimeIt timerB = new();
        timerB.SetDuration(_pauseTime).OnComplete(() => OverTime.Add(lerpC));

        lerpA.OnComplete(() => timerA.Start());
        lerpB.OnComplete(() => timerB.Start());
        lerpC.OnComplete(() => EndClose(useEndB));

        OverTime.Add(lerpA);
    }

    private void EndClose(bool useEndB)
    {
        _state = State.Closed;
        _isTransitioning = false;
        gameObject.SetActive(false);
        if(useEndB)
            closedEndedB?.Invoke();
        else
            closedEnded?.Invoke();
    }

    private void ScaleSetter(Vector3 value)
    {
        transform.localScale = value;
    }

    private void CheckClose()
    {
        Vector3 toCamera = (Camera.main.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(toCamera, transform.forward);
        if (dot < 0)
        {
            Close(false);
        }
    }
}
