using System.Collections.Generic;
using UnityEngine;
using XephTools;

public class Button : MonoBehaviour
{
    [SerializeField] float _pressTime = 0.4f;
    [SerializeField] float _pressAmount = 0.1f;
    [SerializeField] Transform _buttonMesh;

    List<SelfMovable> _movables = new();
    public int signalId = 0;
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;
    Vector3 _startPos = Vector3.zero;

    private void Start()
    {
        SelfMovable[] movables = FindObjectsOfType<SelfMovable>();
        foreach (SelfMovable movable in movables)
        {
            if (movable.signalId == signalId)
                _movables.Add(movable);
        }

        if (_buttonMesh == null)
        {
            Debug.LogError(name + ": Button Mesh not assigned");
            return;
        }
        _startPos = _buttonMesh.localPosition;
    }

    public void Insteract()
    {
        if (_lerpRef != null && !_lerpRef.IsExpired())
            return;

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
        Debug.Log("Button Activate");
        foreach (SelfMovable movable in _movables)
        {
            movable.Toggle();
        }
    }
}