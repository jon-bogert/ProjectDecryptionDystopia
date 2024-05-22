using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    LevelManager _level;

    private void Start()
    {
        _level = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonMovement player = other.GetComponent<ThirdPersonMovement>();
        if (player == null)
            return;

        _level.LevelComplete();
    }
}
