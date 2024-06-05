using TMPro;
using UnityEngine;
using UnityEngine.Events;
using XephTools;

public class Button3D : MonoBehaviour
{
    [SerializeField] float _fadeTime = 0.1f;
    [Space]
    public UnityEvent onPressEvent;

    ButtonBounds _bounds;
    TMP_Text _text = null;
    Material _material;

    public TMP_Text text { get { return _text; } }

    private void Start()
    {
        _bounds = GetComponentInChildren<ButtonBounds>();
        if ( _bounds == null )
        {
            Debug.LogError(name + " (Button 3D): Could not find ButtonBounds Component in Children");
        }
        else
        {
            _bounds.collisionEvent += OnPress;
        }

        _text = GetComponentInChildren<TMP_Text>();
        _material = _bounds.GetComponent<MeshRenderer>().material;

        _material.SetFloat("_Selected", 0f);
    }

    private void OnDestroy()
    {
        if (_bounds)
        {
            _bounds.collisionEvent -= OnPress;
        }
    }

    void OnPress(Collider other)
    {
        _material.SetFloat("_Selected", 1f);

        OverTime.LerpModule lerp = new(
            1f, 0f, _fadeTime,
            (val) => {
                _material.SetFloat("_Selected", val);
            });
        OverTime.Add(lerp);

        onPressEvent?.Invoke();
    }
}
