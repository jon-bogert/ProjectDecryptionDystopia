using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] float _radius;
    [SerializeField] Vector3 _center;
    [SerializeField] LayerMask _collisionMask;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + _center, _radius);
    }

    public bool CheckGround()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position + _center, _radius, _collisionMask);
        return (collisions.Length > 0);
    }
}
