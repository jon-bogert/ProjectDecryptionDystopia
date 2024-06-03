using UnityEngine;

public class SoundFromEvent : MonoBehaviour
{
    SoundPlayer3D _soundPlayer;

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
    }

    public void Play(string key)
    {
        if (key == "")
            return;

        _soundPlayer.Play(key, transform.position);
    }
}
