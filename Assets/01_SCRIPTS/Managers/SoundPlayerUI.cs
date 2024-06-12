using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SoundPlayerUI : MonoBehaviour
{
    [System.Serializable]
    private class _AudioClip
    {
        public string key;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 1f;
    }

    [System.Serializable]
    private class ImplAudioClip
    {
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = 1f;
    }

    public enum Bank { Single, Multi };

    [SerializeField] List<_AudioClip> singleSoundBank/* = new List<_AudioClip>()*/;

    Dictionary<string, ImplAudioClip> _single = new Dictionary<string, ImplAudioClip>();

    AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();

        foreach (_AudioClip clip in singleSoundBank)
        {
            ImplAudioClip c = new ImplAudioClip();
            c.volume = clip.volume;
            c.clip = clip.clip;
            _single.Add(clip.key, c);
        }
        singleSoundBank.Clear();
    }

    public void Play(string key)
    {
        if (_single.ContainsKey(key))
        {
            _source.PlayOneShot(_single[key].clip, _single[key].volume * GameSettings.instance.sfxVolume);
        }
        else
        {
            Debug.LogError("SoundManager -> Could not find " + key + " in sound banks");
        }
    }
}