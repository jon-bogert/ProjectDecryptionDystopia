using UnityEngine;
using XephTools;

public class HitFlash : MonoBehaviour
{
    [SerializeField] float _timeDown = 0.25f;
    [SerializeField] MeshRenderer[] _bodyRenderers;

    float _smoothnessMax = 1.0f;
    OverTime.ModuleReference<OverTime.LerpModule> _lerpRef;

    private void Start()
    {
        _smoothnessMax = _bodyRenderers[0].material.GetFloat("_Smoothness");
    }

    public void PlayHit()
    {
        if (_lerpRef != null && !_lerpRef.IsExpired())
        {
            _lerpRef.Get().End();
        }

        SmoothnessSetter(0f);
        OverTime.LerpModule lerp = new(0f, _smoothnessMax, _timeDown, SmoothnessSetter);
        _lerpRef = OverTime.Add(lerp);
    }

    private void SmoothnessSetter(float value)
    {
        foreach (MeshRenderer renderer in _bodyRenderers)
        {
            renderer.material.SetFloat("_Smoothness", value);
        }
    }
}
