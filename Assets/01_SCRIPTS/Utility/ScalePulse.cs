using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    [SerializeField] float _scaleFactor = 1.5f;
    [SerializeField] float _timeFactor = 1f;

    float _initScale = 1f;
    float _time = 0f;

    private void Awake()
    {
        _initScale = transform.localScale.x;
    }

    private void Update()
    {
        float scaleFinal = Mathf.Sin(_time) * _scaleFactor + _initScale;
        transform.localScale = Vector3.one * scaleFinal;
        _time += Time.deltaTime * _timeFactor;
        while (_time > Mathf.PI * 2f)
        {
            _time -= Mathf.PI * 2f;
        }
    }
}
