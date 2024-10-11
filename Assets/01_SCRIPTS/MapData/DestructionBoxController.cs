using UnityEngine;
using UnityEngine.Events;
using XephTools;

[RequireComponent(typeof(MeshRenderer))]
public class DestructionBoxController : MonoBehaviour
{
    Material _material;

    [SerializeField] float _time = 0.25f;
    [SerializeField] float _aStart = 0f;
    [SerializeField] float _aEnd = 0.55f;
    [SerializeField] float _bStart = -.1f;
    [SerializeField] float _bEnd = 0.55f;
    [SerializeField] GameObject _rootObject;
    [SerializeField] UnityEvent _onDeactiveate;

    bool _hasRun = false;

    SoundPlayer3D _soundPlayer;

    private void Awake()
    {
        if (_rootObject == null)
            Debug.LogError(name + " Destruction Box -> Root Object is not assigned");

        _material = GetComponent<MeshRenderer>().material;

        _material.SetFloat("_TransitionA", _aStart);
        _material.SetFloat("_TransitionB", _bStart);
    }

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    public void Go()
    {
        if (_hasRun)
            return;
        _hasRun = true;

        _soundPlayer.Play("decon-up", transform.position, SoundPlayer3D.Bank.Single);

        OverTime.LerpModule lerpA = new(_aStart, _aEnd, _time, (val) => { _material.SetFloat("_TransitionA", val); });
        lerpA.OnComplete(LaunchB);
        OverTime.Add(lerpA);
    }

    private void LaunchB()
    {
        _soundPlayer.Play("decon-down", transform.position, SoundPlayer3D.Bank.Single);

        _rootObject.SetActive(false);
        _onDeactiveate?.Invoke();
        OverTime.LerpModule lerpB = new(_bStart, _bEnd, _time, (val) => { _material.SetFloat("_TransitionB", val); });
        OverTime.Add(lerpB);
    }
}
