using UnityEngine;
using UnityEngine.Events;
using XephTools;

public class Health : MonoBehaviour
{
    public delegate void HealthCallback(float factor);

    [SerializeField] float _maxHealth = 1f;
    [SerializeField] Transform _targetPoint = null;
    [SerializeField] UnityEvent onDeath;
    [SerializeField] bool _useRegen = false;
    [SerializeField] float _regenDelay = 5f;
    [SerializeField] float _fullRegenTime = 2f;

    [Header("Debug")]
    [SerializeField] bool _isInvincible = false;
    [SerializeField] bool _doLog = false;

    public HealthCallback onHealthChange;

    OverTime.ModuleReference<OverTime.LerpModule> _regenModuleRef = null;
    TimeIt _regenDelayTimer = new();

    float _health = 1f;
    bool _isDead = false;
    float _invMaxHealth = 1f;

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
        if (!Debug.isDebugBuild)
            _isInvincible = false;

        _health = maxHealth;
        _invMaxHealth = 1f / _maxHealth;
    }

    private void Start()
    {
        if (GetComponent<ThirdPersonMovement>() != null) // is the player
        {
            LevelEndManager levelEnd = FindObjectOfType<LevelEndManager>();
            onDeath.AddListener(levelEnd.OnDeath);
        }
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

        onHealthChange?.Invoke(_health * _invMaxHealth);

        if (_doLog)
            Debug.Log(name + " has taken damage: " + _health);

        if (_health <= 0f)
        {
            Kill();
            return;
        }

        if (_useRegen)
        {
            SetRegen();
        }
    }

    public void Kill()
    {
        _health = 0f;
        if (_isInvincible)
            return;

        _isDead = true;
        onDeath?.Invoke();
    }

    public void Heal(float amt)
    {
        _health += amt;
        if (_health > _maxHealth)
            _health = _maxHealth;
    }

    private void SetRegen()
    {
        if (_regenModuleRef != null && !_regenModuleRef.IsExpired())
        {
            _regenModuleRef.Get().End(_health);
        }

        bool wasExpired = _regenDelayTimer.isExpired;
        _regenDelayTimer.SetDuration(_regenDelay);

        float progress = _health * _invMaxHealth;
        float time = (1f - progress) * _fullRegenTime;

        OverTime.LerpModule lerp = new(_health, _maxHealth, time, RegenSetter);

        _regenDelayTimer.OnComplete(() => OverTime.Add(lerp));

        if (wasExpired)
            _regenDelayTimer.Start();
    }

    private void RegenSetter(float val)
    {
        _health = val;
        onHealthChange?.Invoke(val * _invMaxHealth);
    }
}
