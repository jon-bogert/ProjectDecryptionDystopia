using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XephTools;

public class Button : MonoBehaviour
{
    [SerializeField] float _pressTime = 0.4f;
    [SerializeField] float _pressAmount = 0.1f;
    [SerializeField] Transform _buttonMesh;
    [SerializeField] Transform _lineOrigin;

    [Space]
    [SerializeField] UnityEvent _onButtonApproach;
    [SerializeField] UnityEvent _onButtonLeave;

    [Space]
    [SerializeField] GameObject _buttonConnectorLinePrefab;

    List<SelfMovable> _movables = new();
    public int[] signalId = new int[1] { 0 };
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;
    Vector3 _startPos = Vector3.zero;

    SoundPlayer3D _soundPlayer;

    bool CheckSignal(int id)
    {
        foreach (int i in signalId)
        {
            if (i == id)
                return true;
        }
        return false;
    }

    private void Start()
    {
        SelfMovable[] movables = FindObjectsOfType<SelfMovable>();
        foreach (SelfMovable movable in movables)
        {
            if (CheckSignal(movable.signalId))
            {
                _movables.Add(movable);
                ButtonConnectionLines connector = Instantiate(_buttonConnectorLinePrefab, transform).GetComponent<ButtonConnectionLines>();
                connector.origin = _lineOrigin;
                connector.destination = movable.transform;

                DissolveInOutSingle dissolver = connector.GetComponent<DissolveInOutSingle>();

                _onButtonApproach.AddListener(dissolver.DissolveIn);
                _onButtonLeave.AddListener(dissolver.DissolveOut);
            }
        }

        if (_buttonMesh == null)
        {
            Debug.LogError(name + ": Button Mesh not assigned");
            return;
        }
        _startPos = _buttonMesh.localPosition;

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    public void Interact()
    {
        if (_lerpRef != null && !_lerpRef.IsExpired())
            return;

        _soundPlayer.Play("button-press", transform.position, SoundPlayer3D.Bank.Single);

        OverTime.LerpModule lerpDown = new(0, _pressAmount, _pressTime,
            (val) => _buttonMesh.localPosition = new Vector3(
                _startPos.x,
                _startPos.y - val,
                _startPos.z));
        OverTime.LerpModule lerpUp = new(_pressAmount, 0, _pressTime,
            (val) => _buttonMesh.localPosition = new Vector3(
                _startPos.x,
                _startPos.y - val,
                _startPos.z));

        lerpDown.OnComplete(() =>
        {
            Activate();
            _lerpRef = OverTime.Add(lerpUp);
        });

        _lerpRef = OverTime.Add(lerpDown);
    }

    private void Activate()
    {
        foreach (SelfMovable movable in _movables)
        {
            movable.Toggle();
        }
    }

    public void ButtonApproach()
    {
        _onButtonApproach?.Invoke();
    }

    public void ButtonLeave()
    {
        _onButtonLeave?.Invoke();
    }
}
