using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float _maxTime = 5f;
    [SerializeField] float _speed = 5f;
    [SerializeField] float _damage = 0.1f;

    float _timer = 0f;
    Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (_timer < 0)
        {
            End();
            return;
        }

        _timer -= Time.deltaTime;
    }

    public void Fire(Vector3 position, Vector3 direction)
    {
        _timer = _maxTime;
        gameObject.SetActive(true);
        _rigidbody.position = position;
        _rigidbody.velocity = direction * _speed;
    }

    public void End()
    {
        _timer = 0f;
        gameObject.SetActive(false);
        _rigidbody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(_damage);
        }
        End();
    }
}
