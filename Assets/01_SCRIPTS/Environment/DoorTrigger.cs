using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    LevelManager _level;
    bool _hasTriggered = false;

    private void Start()
    {
        _level = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasTriggered)
            return;

        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player == null)
            return;

        _hasTriggered = true;

        _level.LevelComplete();
    }
}
