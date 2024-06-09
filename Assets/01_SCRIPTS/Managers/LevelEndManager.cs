using UnityEngine;
using XephTools;

public class LevelEndManager : MonoBehaviour
{
    public enum Next
    {
        LoadNext,
        LoadNextTutorial,
        StartGameplay,
    }

    [SerializeField] float _dealthDelay = 1f;
    [SerializeField] Next _whatsNext = Next.LoadNext;

    OccludeCamera _cameraOccluder;
    SceneLoader _sceneLoader;
    MenuBoundsAnimator _utilityMenu;

    private void Start()
    {
        _cameraOccluder = FindObjectOfType<OccludeCamera>();
        _sceneLoader = FindObjectOfType<SceneLoader>();
        _utilityMenu = FindObjectOfType<MenuBoundsAnimator>();
    }

    public void OnDeath()
    {
        _cameraOccluder.blockEnd += LoadDeath;
        TimeIt timer = new();
        timer.SetDuration(_dealthDelay).OnComplete(FadeOut).Start();
    }

    public void OnSuccess()
    {
        _cameraOccluder.blockEnd += LoadSuccess;
        FadeOut();
    }

    public void MainMenu()
    {
        _utilityMenu.Close(false);
        _cameraOccluder.blockEnd = () => { _sceneLoader.LoadMainMenu(); };
        MusicManager.Stop();
        FadeOut();
    }

    private void FadeOut()
    {
        _cameraOccluder.Block();
    }

    private void LoadSuccess()
    {
        if (_sceneLoader == null)
            return;

        switch(_whatsNext)
        {
            case Next.LoadNext:
                _sceneLoader.LoadNext();
                break;
            case Next.LoadNextTutorial:
                _sceneLoader.LoadNextIndex();
                break;
            case Next.StartGameplay:
                _sceneLoader.StartGameplay();
                break;
        }
    }

    private void LoadDeath()
    {
        if (_sceneLoader == null)
            return;

        _sceneLoader.Reload();
    }
}
