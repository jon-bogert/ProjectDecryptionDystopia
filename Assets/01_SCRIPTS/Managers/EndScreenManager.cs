using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] Transform _endScreenObject;

    OccludeCamera _occluder;
    SceneLoader _sceneLoader;

    private void Start()
    {
        MusicManager.Stop();

        _occluder = FindObjectOfType<OccludeCamera>();
        _occluder.UnBlock();

        _sceneLoader = SceneLoader.inst;

        Vector3 pos = _endScreenObject.transform.position;
        _endScreenObject.transform.position = new Vector3(pos.x, _sceneLoader.levelY, pos.z);
    }

    public void MainMenu()
    {
        _occluder.blockEnd = () => { _sceneLoader.LoadMainMenu(); };
        _occluder.Block();
    }
}
