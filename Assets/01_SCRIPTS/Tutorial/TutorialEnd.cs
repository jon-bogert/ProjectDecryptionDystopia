using UnityEngine;
using XephTools;

public class TutorialEnd : MonoBehaviour
{

    [SerializeField] float _delayTime = 3f;
    [SerializeField] string _openUiEventName = "Open";

    private void Start()
    {

        UIEventSequencer ui = FindObjectOfType<UIEventSequencer>();

        TimeIt timer = new();
        timer.SetDuration(_delayTime).OnComplete(() => ui.StartSequence(_openUiEventName)).Start();
    }

    public void LoadGameplay()
    {
        SceneLoader.inst.StartGameplay();
    }
}
