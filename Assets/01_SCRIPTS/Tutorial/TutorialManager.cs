using UnityEngine;
using XephTools;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] float _levelStartDelay = 5f;
    [SerializeField] bool _levelEndsByCounter = false;
    [SerializeField] int _levelEndCount = 1;

    int _counter = 0;
    UIEventSequencer _eventSequencer;
    OccludeCamera _occluder;
    LevelEndManager _endManager;

    private void Start()
    {
        _occluder = FindObjectOfType<OccludeCamera>();
        _eventSequencer = FindObjectOfType<UIEventSequencer>();
        _endManager = FindObjectOfType<LevelEndManager>();

        TimeIt timer = new TimeIt();
        timer.SetDuration( _levelStartDelay ).OnComplete(Begin).Start();
    }

    private void Begin()
    {
        _occluder.UnBlock();
        _eventSequencer.StartSequence("desc");
    }

    public void SkipTutorial()
    {
        _occluder.blockEnd = SceneLoader.inst.StartGameplay;
        _occluder.Block();
    }

    public void IncreaseCounter()
    {
        if (_levelEndsByCounter && ++_counter >= _levelEndCount)
            FindObjectOfType<LevelEndManager>().OnSuccess();
    }
}
