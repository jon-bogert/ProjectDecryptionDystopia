using UnityEngine;
using XephTools;

public class EnemyRangeAttack : MonoBehaviour
{
    [SerializeField] float _shotTimeMin = 0.5f;
    [SerializeField] float _shotTimeMax = 2f;
    [Tooltip("How many integral notches between min and max")]
    [SerializeField] int _shotTimeResolution = 4;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _stunTime = 5f;

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

        enabled = false;
    }

    private void Update()
    {
        if (_isStunned)
            return;

        VRDebug.Monitor(4, _timer);
        if (_timer <= 0f)
        {
            RestartTimer();
            Vector3 direction = (_playerHealth.targetPoint - _firePoint.position).normalized;
            _pool.FireNext(_firePoint.position, direction);
        }
        _timer -= Time.deltaTime;
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

    float Remap(int input, int inMin, int inMax, float outMin, float outMax)
    {
        return (input - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    void RestartTimer()
    {
        int rand = Random.Range(0, _shotTimeResolution);
        _timer = Remap(rand, 0, _shotTimeResolution, _shotTimeMin, _shotTimeMax);
    }


}
