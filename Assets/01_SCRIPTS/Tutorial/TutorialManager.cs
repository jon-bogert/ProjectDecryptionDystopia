using UnityEngine;
using XephTools;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] float _levelStartDelay = 5f;
    UIEventSequencer _eventSequencer;
    OccludeCamera _occluder;

    private void Start()
    {
        _occluder = FindObjectOfType<OccludeCamera>();
        _eventSequencer = FindObjectOfType<UIEventSequencer>();

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
}
