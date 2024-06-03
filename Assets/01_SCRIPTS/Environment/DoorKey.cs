using UnityEngine;

public class DoorKey : MonoBehaviour
{
    DoorLock _lock;
    SoundPlayer3D _soundPlayer;

    private void Start()
    {
        _lock = FindObjectOfType<DoorLock>();
        if (_lock == null )
        {
            Debug.LogError(name + " could not find Door in scene");
            return;
        }

        _lock.AddKey(this);

        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player == null)
            return;

        _soundPlayer.Play("key-collect", transform.position, SoundPlayer3D.Bank.Single);
        _lock.RemoveKey(this);
        gameObject.SetActive(false);
    }
}
