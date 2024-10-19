using TMPro;
using UnityEngine;
using XephTools;

[RequireComponent(typeof(LineRenderer))]
public class InteractionLineController : MonoBehaviour
{
    [SerializeField] float _returnTime = 0.2f;

    LineRenderer _lineRenderer;
    Transform _activeTransform = null;

    OverTime.ModuleReference<Vector3Lerp> _lerpRef;

    public bool isEnded { get { return (!_lineRenderer.gameObject.activeSelf) || (_lerpRef != null && !_lerpRef.IsExpired()); } }

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _lineRenderer.gameObject.SetActive(false);
    }

    public void Activate()
    {
        _lineRenderer.gameObject.SetActive(true);
        if (_lerpRef != null && !_lerpRef.IsExpired())
            _lerpRef.Get().End();
    }

    public void UpdateEndPoint(Vector3 pos)
    {
        _lineRenderer.SetPositions(new[] { transform.position, pos });
    }

    public void End()
    {
        if (!_lineRenderer.gameObject.activeSelf)
            return;

        if (_lerpRef != null && !_lerpRef.IsExpired())
            _lerpRef.Get().End();

        Vector3 startPos = _lineRenderer.GetPosition(1);

        Vector3Lerp lerp = new(startPos, transform.position, _returnTime, (Vector3 p) => { _lineRenderer.SetPosition(1, p); });
        lerp.OnComplete(() => { _lineRenderer.gameObject.SetActive(false); });
        _lerpRef = OverTime.Add(lerp);
    }
}
