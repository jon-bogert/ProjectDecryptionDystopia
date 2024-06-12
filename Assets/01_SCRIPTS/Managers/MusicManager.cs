using UnityEngine;
using XephTools;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] bool _playOnAwake = true;
    [SerializeField] float _volume = 0.25f;
    [SerializeField] float _fadeTime = 1.0f;

    static MusicManager instance = null;
    AudioSource _source;

    bool _defferedPlay = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (instance != this)
            return;

        if (_playOnAwake)
            _defferedPlay=true;
    }

    private void Update()
    {
        if (_defferedPlay)
        {
            _defferedPlay = false;
            Play();
        }
    }

    public static void Play()
    {
        OverTime.LerpModule lerp = new(
            0f,
            instance._volume * GameSettings.instance.musicVolume,
            instance._fadeTime,
            (val) => instance._source.volume = val);

        OverTime.Add(lerp);
        instance._source.Play();
    }
    public static void Pause()
    {
        instance._source.Pause();
    }

    public static void UnPause()
    {
        instance._source.UnPause();
    }

    public static void Stop()
    {
        OverTime.LerpModule lerp = new( instance._volume, 0f, instance._fadeTime, (val) => instance._source.volume = val);
        lerp.OnComplete(instance._source.Stop);
        OverTime.Add(lerp);
    }
}
