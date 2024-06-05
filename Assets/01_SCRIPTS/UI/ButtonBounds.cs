using System;
using UnityEngine;
using XephTools;

public class ButtonBounds : MonoBehaviour
{
    [SerializeField] bool _isFresnelShader = false;
    [SerializeField] float _fadeTime = 0.1f;
    [SerializeField] float _powerTarget = 0f;
    [Space]
    public Action<Collider> collisionEvent;

    float _powerStart = 3f;
    Material _material;
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;

    private void Start()
    {
        if (!_isFresnelShader)
            return;

        _material = GetComponent<MeshRenderer>().material;
        _powerStart = _material.GetFloat("_Power");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled)
            return;

        if (!_isFresnelShader)
        {
            collisionEvent?.Invoke(other);
            return;
        }

        if (_lerpRef != null && !_lerpRef.IsExpired())
            return;

        OverTime.LerpModule lerpA = new(_powerStart, _powerTarget, _fadeTime, (val) => _material.SetFloat("_Power", val));
        OverTime.LerpModule lerpB = new(_powerTarget, _powerStart, _fadeTime, (val) => _material.SetFloat("_Power", val));

        lerpA.OnComplete(() => { _lerpRef = OverTime.Add(lerpB); });
        _lerpRef = OverTime.Add(lerpA);

        collisionEvent?.Invoke(other);
    }
}
