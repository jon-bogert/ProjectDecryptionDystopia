using UnityEngine;

public class SoundFromEvent : MonoBehaviour
{
    SoundPlayer3D _soundPlayer;

    [Tooltip("For tutorial visuals")]
    [SerializeField] bool _disableSounds = false;

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    public void Play(string key)
    {
        if (_disableSounds)
            return;

        if (key == "")
            return;

        _soundPlayer.Play(key, transform.position);
    }
}
