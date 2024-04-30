using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public delegate void HurtboxEvent(Collider collision);

    public HurtboxEvent onHurt;

    [SerializeField] LayerMask _collisionLayers = 0;
    [SerializeField] Vector3 _dimensions = Vector3.one;
    [Tooltip("Don't set to non-zero value if calling HurtEnd()")]
    [SerializeField] float _length = 0f;

    bool _isHurting = false;
    float _timer = 0f;
    HashSet<Collider> _colliderBuffer = new HashSet<Collider>();

    public bool isHurting { get { return _isHurting; } }

    private void Awake()
    {
        if (_collisionLayers == 0)
            Debug.LogError(name + " Hurtbox collision layers set to None");
    }

    private void Update()
    {
        if (!_isHurting)
            return;

        if (_length > 0f)
        {
            if (_timer <= 0f)
            {
                HurtEnd();
                return;
            }

            _timer -= Time.deltaTime;
        }

        Collider[] newCollisions = Physics.OverlapBox(transform.position, _dimensions * 0.5f, transform.rotation, _collisionLayers);

        for (int i = 0; i < newCollisions.Length; i++)
        {
            if (_colliderBuffer.Contains(newCollisions[i]))
                continue;

            _colliderBuffer.Add(newCollisions[i]);
            onHurt?.Invoke(newCollisions[i]);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = (_isHurting) ? new Color(1f, 0f, 0f, 0.5f) : new Color(0f, 1f, 1f, 0.5f) ;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, _dimensions);
        Gizmos.matrix = Matrix4x4.identity;
    }

    //Call at the beginning of your attack animation
    public void HurtStart()
    {
        if (_isHurting)
        {
            Debug.LogError(name + " Hurtbox.HurtStart() -> must call HurtEnd() first");
            return;
        }
        Debug.Log("Hurt Start");
        _isHurting = true;

        if (_length <= 0)
            return;

        _timer = _length;
    }

    //Call at the end of your attack animation
    public void HurtEnd()
    {
        Debug.Log("HurtEnd");
        _isHurting = false;
        _colliderBuffer.Clear();
    }
}
