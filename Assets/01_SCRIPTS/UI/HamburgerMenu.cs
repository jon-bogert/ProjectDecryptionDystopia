using UnityEngine;

public class HamburgerMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ButtonBounds _toggleButton;
    [SerializeField] MenuBoundsAnimator _menuBounds;

    public bool isOpen { get { return _menuBounds.state == MenuBoundsAnimator.State.Open; } }

    private void Start()
    {
        _toggleButton.collisionEvent += OnToggle;
    }

    private void OnDestroy()
    {
        if (_toggleButton != null)
            _toggleButton.collisionEvent -= OnToggle;
    }

    void OnToggle(Collider other)
    {
        if (_menuBounds.state == MenuBoundsAnimator.State.Open)
            _menuBounds.Close(false);
        else
        {
            _menuBounds.transform.LookAt(Camera.main.transform.position);
            _menuBounds.Open();
        }
    }
}
