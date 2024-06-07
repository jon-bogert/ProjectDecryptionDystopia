using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader inst = null;

    [Header("Debug Parameters")]
    [SerializeField] bool _kioskMode = false;
    [SerializeField] int _level = -1;
    [SerializeField] SceneType _sceneType = SceneType.Generic;
    [SerializeField] float _levelY = 23f;
    [SerializeField] float _levelHeight = 12f;

    public enum SceneType
    {
        Generic,
        CoreLevel,
        CustomLevel,
    }


    string[] _coreLevels = null;
    //string[] _customLevels = null;

    public int level { get { return _level; } }
    public float levelY { get { return _levelY; } }

    private void Awake()
    {
        if (inst != null)
        {
            Destroy(gameObject);
            return;
        }

        inst = this;
        DontDestroyOnLoad(gameObject);

        if (!Directory.Exists("core"))
            Directory.CreateDirectory("core");
        if (!Directory.Exists("custom"))
            Directory.CreateDirectory("custom");

        _coreLevels = System.IO.Directory.GetFiles("core");
        //_customLevels = System.IO.Directory.GetFiles("custom");
    }

    public void StartGameplay()
    {
        _sceneType = SceneType.CoreLevel;
        SceneManager.LoadScene("Gameplay");
    }

    public void Reload()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void LoadNext()
    {
        if (_sceneType == SceneType.Generic)
        {
            Debug.LogError("Load Scene should not be called when scene type is Generic");
            return;
        }

        if (_sceneType == SceneType.CustomLevel)
        {
            LoadMainMenu();
            return;
        }

        if (_level >= _coreLevels.Length - 1)
        {
            LoadEnd();
            return;
        }
        ++_level;
        SceneManager.LoadScene("Gameplay");
    }

    public void LoadEnd()
    {
        _level = 0;
        _sceneType = SceneType.Generic;
        SceneManager.LoadScene("End");
    }

    public void LoadMainMenu()
    {
        if (_kioskMode)
            _level = 0;

        _sceneType = SceneType.Generic;
        SceneManager.LoadScene("MainMenu");
    }

    public string GetLevelFile()
    {
        if (_sceneType == SceneType.Generic)
        {
            Debug.LogError("GetLevelFile should not be called when scene type is Generic");
            return "";
        }

        if (_sceneType == SceneType.CustomLevel)
        {
            Debug.LogError("Loading Custom Levels not Implemented");
            return "";
        }

        return _coreLevels[_level];
    }

    public void MeasureLevelY()
    {
        _levelY = Camera.main.transform.position.y - _levelHeight;
    }
}
