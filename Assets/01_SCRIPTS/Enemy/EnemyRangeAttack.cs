using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour
{
    [SerializeField] float _shotTimeMin = 0.5f;
    [SerializeField] float _shotTimeMax = 2f;
    [Tooltip("How many integral notches between min and max")]
    [SerializeField] int _shotTimeResolution = 4;
    [SerializeField] Transform _firePoint;

    ProjectilePool _pool;
    ThirdPersonMovement _player;
    float _time = 0f;
    float _timer = 0f;

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

        _player = FindObjectOfType<ThirdPersonMovement>();
        if (_player == null )
        {
            Debug.LogError("Could not find player in scene");
        }

        enabled = false;
    }

    private void Update()
    {
        if (_timer <= 0f)
        {
            RestartTimer();
            Vector3 direction = (_firePoint.position - _player.transform.position).normalized;
            _pool.FireNext(_firePoint.position, direction);
        }

        _timer += Time.deltaTime;
    }

    float Remap(int input, int inMin, int inMax, float outMin, float outMax)
    {
        return (input - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    void RestartTimer()
    {
        int rand = Random.Range(0, _shotTimeResolution);
        _time = Remap(rand, 0, _shotTimeResolution, _shotTimeMin, _shotTimeMax);
    }


}
