using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] Animator _legAnimator;
    [SerializeField] Animator _armAnimator;

    SoundPlayer3D _soundPlayer;

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    public void Invoke()
    {
        _soundPlayer.Play("death", transform.position, SoundPlayer3D.Bank.Single);

        _legAnimator.speed = 1f;
        _armAnimator.speed = 1f;

        _legAnimator.SetTrigger("OnDeath");
        _armAnimator.SetTrigger("OnDeath");
    }
}
