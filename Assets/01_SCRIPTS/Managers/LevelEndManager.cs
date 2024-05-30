using UnityEngine;
using XephTools;

public class LevelEndManager : MonoBehaviour
{
    [SerializeField] float _dealthDelay = 1f;
    OccludeCamera _cameraOccluder;

    private void Start()
    {
        _cameraOccluder = FindObjectOfType<OccludeCamera>();
    }

    public void OnDeath()
    {
        TimeIt timer = new();
        timer.SetDuration(_dealthDelay).OnComplete(FadeOut).Start();
    }

    public void OnSuccess()
    {
        FadeOut();
    }

    private void FadeOut()
    {
        _cameraOccluder.Block();
    }
}
