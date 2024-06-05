using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XephTools;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerMovableSound))]
public class EnemyStunner : MonoBehaviour
{
    [SerializeField] UnityEvent _stunEvent;
    [Range(0f, 1f)]
    [SerializeField] float _hapticIntensity = 0.7f;
    [SerializeField] float _hapticDuration = 0.01f;
    [SerializeField] float _stunTime = 5f;
    [SerializeField] float _stunBrightness = 2f;

    [Header("References")]
    [SerializeField] MeshRenderer _hoverMesh;

    List<InteractionPoint> _interactors = new List<InteractionPoint>();
    Health _health;
    PlayerMovableSound _stunSound;
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef = null;

    Color _hoverColor = Color.white;
    bool _isAnimating = false;

    private void Start()
    {
        _hoverMesh.enabled = false;
        _health = GetComponent<Health>();
        _hoverColor = _hoverMesh.material.GetColor("_Color");
        _stunSound = GetComponent<PlayerMovableSound>();
    }

    private void Update()
    {
        if (_health.isDead && _interactors.Count > 0)
        {
            _interactors.Clear();
            _hoverMesh.enabled = false;
        }

        foreach (InteractionPoint interactionPoint in _interactors)
        {
            if (interactionPoint.isPressed)
            {
                interactionPoint.SendHaptic(_hapticIntensity, _hapticDuration);
                _stunEvent?.Invoke();
                Animate();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;

        if (_interactors.Count == 0)
            _hoverMesh.enabled = true;
        
        interactionPoint.SendHaptic(_hapticIntensity, _hapticDuration);
        _interactors.Add(interactionPoint);
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionPoint interactionPoint = other.GetComponent<InteractionPoint>();
        if (interactionPoint == null)
            return;

        _interactors.Remove(interactionPoint);

        if (_interactors.Count == 0 && !_isAnimating)
            _hoverMesh.enabled = false;
    }

    private void Animate()
    {
        _isAnimating = true;

        if (_lerpRef != null && !_lerpRef.IsExpired())
        {
            _lerpRef.Get().End(_lerpRef.Get().Progress);
        }

        _stunSound.On();
        Color startColor = Color.white * _stunBrightness;
        _hoverMesh.material.SetColor("_Color", startColor);
        OverTime.LerpModule lerp = new(1f, 0f, _stunTime, (val) => _hoverMesh.material.SetFloat("_Brightness", val));

        lerp.OnComplete(ResetVals);
        _lerpRef = OverTime.Add(lerp);
    }

    private void ResetVals()
    {
        _hoverMesh.material.SetColor("_Color", _hoverColor);
        _hoverMesh.material.SetFloat("_Brightness", 1f);
        _isAnimating = false;
        _hoverMesh.enabled = false;
        _stunSound.Off();
    }


}
