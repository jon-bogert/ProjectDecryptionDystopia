using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] bool _playOnAwake = true;

    static MusicManager instance = null;
    AudioSource _source;

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
            _source.Play();
    }

    public static void Play()
    {
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
        instance._source.Stop();
    }
}
