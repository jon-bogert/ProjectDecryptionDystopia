using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Vector3 _squishCenter = Vector3.zero;
    [SerializeField] Vector3 _squishExtends = Vector3.one;
    [SerializeField] LayerMask _squishMask = 0;
    [SerializeField] float _fallDeathHeight = 5f;
    [Header("References")]
    [SerializeField] Animator _animator;

    SoundPlayer3D _soundPlayer;
    Health _health;

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");

        _health = GetComponent<Health>();
        if (_health == null)
            Debug.LogError("Could not get Health Component");

        if (_squishMask == 0)
            Debug.LogWarning("PlayerDeathHandler: Squish Mask is set to None");
    }

    private void Update()
    {
        if (_health.isDead)
            return;

        if (CheckSquish() || transform.position.y < _fallDeathHeight)
            _health.Kill();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + _squishCenter, _squishExtends);
    }

    private bool CheckSquish()
    {
        Collider[] result = Physics.OverlapBox(
            transform.position + _squishCenter,
            _squishExtends * 0.5f,
            Quaternion.identity,
            _squishMask);

        if (result.Length > 0)
        {
            return true;
        }
        return false;
    }

    public void Invoke()
    {
        _soundPlayer.Play("death", transform.position, SoundPlayer3D.Bank.Single);

        _animator.speed = 1f;

        _animator.SetTrigger("OnDeath");
    }
}
