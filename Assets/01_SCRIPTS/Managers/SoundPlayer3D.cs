using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using static SoundPlayer3D;


[System.Serializable]
public class MultiAudioClip
{
    public AudioClip[] bank;
    int last = -1;
    [Range(0f, 1f)]
    public float volume = .75f;
    public AudioClip GetRandomClip()
    {
        int index = -1;
        do
        {
            index = Random.Range(0, bank.Length);
        } while (index == last);

        return bank[index];
    }
}

public class SoundPlayer3D : MonoBehaviour
{
    [System.Serializable]
    private class _AudioClip
    {
        public string key;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = .75f;
    }

    [System.Serializable]
    private class _MultiAudioClip
    {
        public string key;
        public MultiAudioClip bank;
    }

    [System.Serializable]
    private class ImplAudioClip
    {
        public AudioClip clip;
        [Range(0, 1)]
        public float volume = .75f;
    }

    public enum Bank { Single, Multi };

    [SerializeField] InputActionReference _headsetActive;

    [SerializeField] List<_AudioClip> singleSoundBank/* = new List<_AudioClip>()*/;
    [SerializeField] List<_MultiAudioClip> multiSoundBank = new List<_MultiAudioClip>();

    [SerializeField] int _poolSize = 10;

    Dictionary<string, MultiAudioClip> _multi = new Dictionary<string, MultiAudioClip>();
    Dictionary<string, ImplAudioClip> _single = new Dictionary<string, ImplAudioClip>();

    AudioSource[] _sources;
    int _nextSourceIndex = 0;

    AudioSource _nextSource
    {
        get
        {
            AudioSource next = _sources[_nextSourceIndex];
            _nextSourceIndex = (_nextSourceIndex + 1) % _poolSize;
            return next;
        }
    }

    private void Awake()
    {
        _headsetActive.action.performed += HeadsetActive;

        _sources = new AudioSource[_poolSize];
        for (int i = 0; i < _poolSize; ++i)
        {
            GameObject go = new GameObject("AudioSource_" + i);
            _sources[i] = go.AddComponent<AudioSource>();
            _sources[i].transform.parent = transform;
            _sources[i].volume = 0.5f;
            _sources[i].playOnAwake = false;
            _sources[i].loop = false;
            _sources[i].spatialBlend = 1f;
        }

        foreach (_AudioClip clip in singleSoundBank)
        {
            ImplAudioClip c = new ImplAudioClip();
            c.volume = clip.volume;
            c.clip = clip.clip;
            _single.Add(clip.key, c);
        }
        singleSoundBank.Clear();

        foreach (_MultiAudioClip mclip in multiSoundBank)
        {
            _multi.Add(mclip.key, mclip.bank);
        }
        multiSoundBank.Clear();
    }

    private void OnDestroy()
    {
        _headsetActive.action.performed -= HeadsetActive;
    }

    public void Play(string key, Vector3 position)
    {
        if (key == "")
        {
            Debug.Log("Break");
        }
        AudioSource source = _nextSource;
        source.transform.position = position;
        if (_single.ContainsKey(key))
        {
            source.clip = _single[key].clip;
            source.volume = _single[key].volume;
            source.Play();
        }
        else if (_multi.ContainsKey(key))
        {
            AudioClip clip = _multi[key].GetRandomClip();
            source.clip = clip;
            source.volume = _multi[key].volume;
            source.Play();
        }
        else
        {
            Debug.LogError("SoundManager -> Could not find " + key + " in sound banks");
        }
    }

    public void Play(string key, Vector3 position, Bank bank)
    {
        AudioSource source = _nextSource;
        source.transform.position = position;
        if (bank == Bank.Single && _single.ContainsKey(key))
        {
            source.clip = _single[key].clip;
            source.volume = _single[key].volume;
            source.Play();
            return;
        }
        else if (bank == Bank.Single)
        {
            Debug.LogError("SoundManager -> Could not find " + key + " in single sound bank");
            return;
        }

        if (bank == Bank.Multi && _multi.ContainsKey(key))
        {
            AudioClip clip = _multi[key].GetRandomClip();
            source.clip = clip;
            source.volume = _multi[key].volume;
            source.Play();
        }
        else
        {
            Debug.LogError("SoundManager -> Could not find " + key + " in multi sound bank");
        }
    }

    public float GetLengthOfSingle(string key)
    {
        if (_single.ContainsKey(key))
        {
            return _single[key].clip.length;
        }
        Debug.LogError("SoundManager -> Could not find " + key + " in single sound bank");
        return 0f;
    }

    private void HeadsetActive(InputAction.CallbackContext ctx)
    {
        Debug.Log("BAM");
        foreach (AudioSource source in _sources)
        {
            source.mute = ctx.ReadValue<float>() > 0.5f;
        }
    }
}