using System;
using UnityEngine;
using XephTools;

public class ButtonBounds : MonoBehaviour
{
    [SerializeField] bool _isFresnelShader = false;
    [SerializeField] float _fadeTime = 0.1f;
    [SerializeField] float _powerTarget = 0f;
    [SerializeField] bool _isActive = true;
    [Space]
    public Action<Collider> collisionEvent;

    float _powerStart = 3f;
    Material _material;
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;
    SoundPlayerUI _sounds;

    public bool isActive { get { return _isActive; } set { _isActive = value; } }

    private void Start()
    {
        _sounds = FindObjectOfType<SoundPlayerUI>();
        if (_sounds == null)
        {
            Debug.LogWarning("UI Sounds Player wasn't found in current scene");
        }

        if (!_isFresnelShader) //---------------------------
            return;

        _material = GetComponent<MeshRenderer>().material;
        _powerStart = _material.GetFloat("_Power");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || !_isActive)
            return;

        if (!_isFresnelShader)
        {
            _sounds.Play("button-press");
            collisionEvent?.Invoke(other);
            return;
        }

        if (_lerpRef != null && !_lerpRef.IsExpired())
            return;

        OverTime.LerpModule lerpA = new(_powerStart, _powerTarget, _fadeTime, (val) => _material.SetFloat("_Power", val));
        OverTime.LerpModule lerpB = new(_powerTarget, _powerStart, _fadeTime, (val) => _material.SetFloat("_Power", val));

        lerpA.OnComplete(() => { _lerpRef = OverTime.Add(lerpB); });
        _lerpRef = OverTime.Add(lerpA);

        _sounds?.Play("button-press");

        collisionEvent?.Invoke(other);
    }
}
