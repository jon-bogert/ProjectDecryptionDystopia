using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    [SerializeField] int _numDots = 3;
    [SerializeField] float _speed = .25f;

    bool _isGoing = false;
    float _timer = 0f;

    TMP_Text _field;
    int _count = 0;

    private void Awake()
    {
        _field = GetComponent<TMP_Text>();
    }

    public void Begin()
    {
        _isGoing = true;
    }

    public void Update()
    {
        if (!_isGoing)
            return;

        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            return;
        }

        _timer = _speed;

        string dots = string.Empty;
        for (int i  = 0; i <= _count; i++)
        {
            dots += ".";
        }
        _count = (_count + 1) % _numDots;

        _field.text = "Loading" + dots;
    }
}
