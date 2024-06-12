using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class StepsAudioPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] _audioClips = new AudioClip[0];

    AudioSource _source;
    int _last;
    float _volume;

    private void Awake()
    {
        if (_audioClips.Length <= 0)
        {
            Debug.LogError("StepsAudioPlayer: No Audio Clips Assigned");
        }

        _source = GetComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = 1f;
        _volume = _source.volume;
        _source.volume = _volume * GameSettings.instance.sfxVolume;
    }

    public void Play()
    {
        int index = _last;
        while (index == _last)
        {
            index = Random.Range(0, _audioClips.Length);
        }
        _last = index;

        _source.PlayOneShot(_audioClips[index]);
    }
}
