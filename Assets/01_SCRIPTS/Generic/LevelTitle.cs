using System;
using TMPro;
using UnityEngine;
using XephTools;

public class LevelTitle : MonoBehaviour
{
    Transform _camera;

    [Tooltip("This should be turned on for the tutorials")]
    [SerializeField] bool _keepDefaultText = false;

    [Header("Timing")]
    [SerializeField] float _delayTime = 1f;
    [SerializeField] float _fadeTime = 2f;
    [SerializeField] float _holdTime = 2f;

    [Header("Positioning")]
    [SerializeField] float _distance = 10f;
    [SerializeField] float _smoothTime = 1f;

    [Header("References")]
    [SerializeField] TMP_Text _levelText;
    [SerializeField] TMP_Text _nameText;

    Color whiteClear = new Color(1f, 1f, 1f, 0f);
    Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _camera = Camera.main.transform;
        transform.position = _camera.position + (_camera.forward * _distance);

        ColorSet(whiteClear);

        ColorLerp lerpUp = new(whiteClear, Color.white, _fadeTime, ColorSet);
        lerpUp.OnComplete(LaunchB);
        TimeIt timer = new();
        timer.SetDuration(_delayTime).OnComplete(() => OverTime.Add(lerpUp, _delayTime)).Start();

        if (!_keepDefaultText)
        {
            string levelName = FindObjectOfType<LevelManager>().levelName;
            int levelNum = FindObjectOfType<SceneLoader>().level;

            InitText(levelNum, levelName);
        }
    }

    private void Update()
    {
        Vector3 target = _camera.position + (_camera.forward * _distance);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, _smoothTime);
        transform.LookAt(_camera);
    }

    private void LaunchB()
    {
        ColorLerp lerpDown = new(Color.white, whiteClear, _fadeTime, ColorSet);
        lerpDown.OnComplete(() =>
        {
            try
            {
                gameObject.SetActive(false);
            }
            catch (Exception) { } // Object may not exist
        });
        TimeIt timer = new();
        timer.SetDuration(_holdTime).OnComplete(() => OverTime.Add(lerpDown)).Start();

    }

    private void ColorSet(Color color)
    {
        _levelText.color = color;
        _nameText.color = color;
    }

    public void InitText(int levelNum, string levelName)
    {
        if (levelNum < 0)
        {
            _levelText.text = "CUSTOM_LEVEL";
        }
        else
        {
            _levelText.text = "CIPHER_" + levelNum.ToString("D2");
        }

        _nameText.text = levelName;
    }
}
