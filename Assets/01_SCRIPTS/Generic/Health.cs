using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float _maxHealth = 1f;
    [SerializeField] Transform _targetPoint = null;
    [SerializeField] UnityEvent onDeath;

    [Header("Debug")]
    [SerializeField] bool _doLog = false;

    float _health = 1f;
    bool _isDead = false;

    public float health { get { return _health; } }
    public float maxHealth { get { return _maxHealth; } }
    public bool isDead { get { return _isDead;} }

    public Vector3 targetPoint
    {
        get
        {
            if (_targetPoint == null)
                return transform.position;
            return _targetPoint.position;
        }
    }

    private void Awake()
    {
        _health = maxHealth;
    }

    public void UnDead()
    {
        _health = _maxHealth;
        _isDead = false;
    }

    public void TakeDamage(float amt)
    {
        if (_isDead)
            return;

        _health -= amt;

        if (_doLog)
            Debug.Log(name + " has taken damage: " + _health);

        if (_health > 0f)
            return;

        _isDead = true;
        _health = 0f;
        onDeath?.Invoke();
    }

    public void Heal(float amt)
    {
        _health += amt;
        if (_health > _maxHealth)
            _health = _maxHealth;
    }
}
