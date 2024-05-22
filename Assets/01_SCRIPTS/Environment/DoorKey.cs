using UnityEngine;

public class DoorKey : MonoBehaviour
{
    DoorLock _lock;

    private void Start()
    {
        _lock = FindObjectOfType<DoorLock>();
        if (_lock == null )
        {
            Debug.LogError(name + " could not find Door in scene");
            return;
        }

        _lock.AddKey(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player == null)
            return;

        _lock.RemoveKey(this);
        gameObject.SetActive(false);
    }
}
