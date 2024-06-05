using Unity.VisualScripting;
using UnityEngine;

public class HamburgerMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ButtonBounds _toggleButton;
    [SerializeField] GameObject _menuContents;

    bool _isPaused = false;

    private void Start()
    {
        _toggleButton.collisionEvent += OnToggle;
        Hide();
    }

    private void OnDestroy()
    {
        if (_toggleButton != null)
            _toggleButton.collisionEvent -= OnToggle;
    }

    void OnToggle(Collider other)
    {
        _isPaused = !_isPaused;

        if (_isPaused)
            Show();
        else
            Hide();
    }

    private void Show()
    {
        _menuContents.SetActive(true);
    }

    private void Hide()
    {
        _menuContents.SetActive(false);
    }
}
