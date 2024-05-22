using UnityEngine;

public class KeyBob : MonoBehaviour
{
    [SerializeField] float _bobAmount = 0.1f;
    [SerializeField] float _bobSpeedFactor = 0.1f;
    [SerializeField] float _rotationSpeed = 1f;

    Vector3 _startPos = Vector3.zero;
    float _t = 0f;
    float _rotation = 0f;

    private void Awake()
    {
        _startPos = transform.position;
        _t = Random.Range(0, Mathf.PI * 2f);
        _rotation = Random.Range(0f, 360f);
    }

    private void Update()
    {
        Vector3 offset = Vector3.up * Mathf.Sin(_t) * _bobAmount;
        transform.position = _startPos + offset;

        _t += Time.deltaTime * _bobSpeedFactor;
        while (_t >= Mathf.PI * 2f)
        {
            _t -= Mathf.PI * 2f;
        }

        transform.rotation = Quaternion.Euler(Vector3.up * _rotation);
        _rotation += _rotationSpeed * Time.deltaTime;

        while (_rotation >= 360f)
        {
            _rotation -= 360f;
        }
    }
}
