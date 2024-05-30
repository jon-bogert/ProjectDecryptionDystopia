using UnityEngine;
using XephTools;

public class EnemyRangeAttack : MonoBehaviour
{
    [SerializeField] float _shotTimeMin = 0.5f;
    [SerializeField] float _shotTimeMax = 2f;
    [Tooltip("How many integral notches between min and max")]
    [SerializeField] int _shotTimeResolution = 4;
    [SerializeField] float _stunTime = 5f;
    [Header("References")]
    [SerializeField] Transform _firePoint;
    [SerializeField] Animator _armAnimator;

    ProjectilePool _pool;
    Health _playerHealth;
    float _time = 0f;
    float _timer = 0f;
    TimeIt _stunTimer = new();
    bool _isStunned = false;

    private void Awake()
    {
        _time = _shotTimeMax;
    }

    private void Start()
    {
        _pool = FindObjectOfType<ProjectilePool>();
        if (_pool == null)
        {
            Debug.LogError("Could not find projecile pool in scene");
        }

        _playerHealth = FindObjectOfType<ThirdPersonMovement>().GetComponent<Health>();

        if (_playerHealth == null )
        {
            Debug.LogError("Could not find player in scene");
        }

        if (_armAnimator == null)
            Debug.LogError(name + ": Arm Animator not assigned in inspector");

        enabled = false;
    }

    private void Update()
    {
        if (_isStunned)
            return;

        if (_timer <= 0f)
        {
            RestartTimer();
            transform.LookAt(new Vector3(_playerHealth.targetPoint.x, transform.position.y, _playerHealth.targetPoint.z));
            _armAnimator.SetTrigger("DoAttackRanged");
        }
        _timer -= Time.deltaTime;
    }

    public void ThrowProjectile()
    {
        Vector3 direction = (_playerHealth.targetPoint - _firePoint.position).normalized;
        _pool.FireNext(_firePoint.position, direction);
    }

    public void Stun()
    {
        _isStunned = true;

        if (_stunTimer.isExpired)
        {
            _stunTimer.OnComplete(() => { _isStunned = false; }).SetDuration(_stunTime).Start();
        }
        else
        {
            _stunTimer.SetDuration(_stunTime);
        }
    }

    

    void RestartTimer()
    {
        int rand = Random.Range(0, _shotTimeResolution);
        _timer = XephMath.Remap(rand, 0, _shotTimeResolution, _shotTimeMin, _shotTimeMax);
    }


}
