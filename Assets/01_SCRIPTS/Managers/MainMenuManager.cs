using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Transform _mainMenuObject;

    OccludeCamera _occluder;
    SceneLoader _sceneLoader;

    public void Start()
    {
        _sceneLoader = SceneLoader.inst;
        _occluder = FindObjectOfType<OccludeCamera>();
        _occluder.UnBlock();
    }

    public void End()
    {
        MusicManager.Play();
        _sceneLoader.MeasureLevelY();
        _occluder.blockEnd = () => { _sceneLoader.StartTutorial(); };
        _occluder.Block();
    }

    public void MeasureLevelY()
    {
        _sceneLoader.MeasureLevelY();
        Vector3 pos = _mainMenuObject.transform.position;
        _mainMenuObject.transform.position = new Vector3(pos.x, _sceneLoader.levelY, pos.z);
    }
}
