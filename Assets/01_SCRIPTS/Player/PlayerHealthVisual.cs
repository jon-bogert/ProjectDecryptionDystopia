using UnityEngine;

public class PlayerHealthVisual : MonoBehaviour
{
    [SerializeField] Color _endColor = Color.red;
    [SerializeField] MeshRenderer[] _renderers = new MeshRenderer[0];

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
    }
}