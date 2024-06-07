using UnityEngine;

public class InitScene : MonoBehaviour
{
    private void Start()
    {
        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        //sceneLoader.MeasureLevelY(); // not currently working
        sceneLoader.LoadMainMenu();
    }
}
