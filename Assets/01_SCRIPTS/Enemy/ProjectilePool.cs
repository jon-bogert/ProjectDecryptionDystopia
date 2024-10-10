using UnityEngine;

//NOTE: Only One per scene
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] int _size = 10;

    Projectile[] _pool = null;
    int _next = 0;

    private void Awake()
    {
        _pool = new Projectile[_size];
        for (int i = 0; i < _size; i++)
        {
            _pool[i] = Instantiate(_projectilePrefab).GetComponent<Projectile>();
            _pool[i].gameObject.SetActive(false);
        }
    }

    public Projectile GetNext()
    {
        Projectile p = _pool[_next++];
        if (_next >= _size)
            _next = 0;

        return p;
    }
}
