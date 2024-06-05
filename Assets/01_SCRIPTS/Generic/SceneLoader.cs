using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _inst = null;

    [Header("Debug Parameters")]
    [SerializeField] bool _kioskMode = false;
    [SerializeField] int _level = -1;
    [SerializeField] SceneType _sceneType = SceneType.Generic;

    public enum SceneType
    {
        Generic,
        CoreLevel,
        CustomLevel,
    }


    string[] _coreLevels = null;
    //string[] _customLevels = null;

    public int level { get { return _level; } }

    private void Awake()
    {
        if (_inst != null)
        {
            Destroy(gameObject);
            return;
        }

        _inst = this;
        DontDestroyOnLoad(gameObject);

        if (!Directory.Exists("core"))
            Directory.CreateDirectory("core");
        if (!Directory.Exists("custom"))
            Directory.CreateDirectory("custom");

        _coreLevels = System.IO.Directory.GetFiles("core");
        //_customLevels = System.IO.Directory.GetFiles("custom");
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
}
