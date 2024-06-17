using UnityEngine;
using XephTools;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] float _levelStartDelay = 5f;
    [SerializeField] bool _levelEndsByCounter = false;
    [SerializeField] int _levelEnemyCount = 1;
    [SerializeField] int _levelPlatformCount = 1;

    int _enemyCounter = 0;
    int _platformCounter = 0;
    UIEventSequencer _eventSequencer;
    OccludeCamera _occluder;
    LevelEndManager _endManager;

    bool _hasTriggered = false;

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

    public void IncreaseEnemyCounter()
    {
        ++_enemyCounter;
        CheckEnd();
    }

    public void IncreasePlatformCounter()
    {
        ++_platformCounter;
        CheckEnd();
    }

    private void CheckEnd()
    {
        if (_hasTriggered)
            return;
        if (_levelEndsByCounter && _enemyCounter >= _levelEnemyCount && _platformCounter >= _levelPlatformCount)
        {
            _hasTriggered = true;
            FindObjectOfType<LevelEndManager>().OnSuccess();
        }
    }
}
