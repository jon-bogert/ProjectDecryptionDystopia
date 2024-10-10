using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PlayerHealthVisual : MonoBehaviour
{
    [SerializeField] float _chromaticAbberationIntensity = 1f;
    [SerializeField] Color _endColor = Color.red;
    [SerializeField] MeshRenderer[] _renderers = new MeshRenderer[0];

    Volume _postProcVolume;
    ChromaticAberration _chrAbb;

    Color _startColor = Color.white;
    Health _health;

    private void Start()
    {
        if (_renderers.Length <= 0)
        {
            Debug.LogError("PlayerHealthVisual: 'Renderers' is empty");
            return;
        }
        _startColor = _renderers[0].material.color;
        _health = GetComponent<Health>();
        if (_health == null )
        {
            Debug.LogError("PlayerHealthVisual: Could not find Health component");
            return;
        }
        _health.onHealthChange += UpdateVisual;

        _postProcVolume = FindObjectOfType<Volume>();
        if (_postProcVolume == null )
        {
            Debug.LogError("PlayerHealthVisual: Could not find Post Processing Volume in scene");
            return;
        }

        if (_postProcVolume.profile.TryGet(out ChromaticAberration ca))
            _chrAbb = ca;
    }

    private void OnDestroy()
    {
        if (!_health)
            return;

        _health.onHealthChange -= UpdateVisual;
    }

    private void UpdateVisual(float factor)
    {
        Color colorFinal = Color.Lerp(_endColor, _startColor, factor);
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.material.color = colorFinal;
        }

        if (!_postProcVolume)
            return;

        _chrAbb.intensity.value = (1f - factor) * _chromaticAbberationIntensity;
    }
}