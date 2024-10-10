using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class ButtonConnectionLines : MonoBehaviour
{
    public Transform origin;
    public Transform destination;

    LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, origin.position);
        _lineRenderer.SetPosition(1, destination.position);
    }
}
