using UnityEngine;
using XephTools;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float _maxTime = 5f;
    [SerializeField] float _speed = 5f;
    [SerializeField] float _damage = 0.1f;
    [SerializeField] float _expandTime = 1f;

    float _timer = 0f;
    Rigidbody _rigidbody;
    SoundPlayer3D _soundPlayer;

    Transform _trackingPoint = null;

    bool _isFiring = false;

    Vector3 _finalScale;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _finalScale = transform.localScale;
    }

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }


    private void Update()
    {
        if (_trackingPoint)
            transform.position = _trackingPoint.position;

        if (!_isFiring)
            return;

        if (_timer < 0)
        {
            End();
            return;
        }

        _timer -= Time.deltaTime;
    }

    public void Begin(Transform newParent)
    {
        gameObject.SetActive(true);
        _trackingPoint = newParent;
        transform.position = _trackingPoint.position;
        transform.localScale = Vector3.zero;
        Vector3Lerp lerp = new(Vector3.zero, _finalScale, _expandTime, (Vector3 v) => { transform.localScale = v; });
        OverTime.Add(lerp);
    }

    public void Fire(Vector3 position, Vector3 direction)
    {
        _isFiring = true;
        _trackingPoint = null;
        _timer = _maxTime;
        _rigidbody.position = position;
        _rigidbody.velocity = direction * _speed;
    }

    public void End()
    {
        _timer = 0f;
        _isFiring = false;
        gameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(_damage);
        }
        _soundPlayer.Play("ranged-hit", transform.position, SoundPlayer3D.Bank.Single);
        End();
    }
}
