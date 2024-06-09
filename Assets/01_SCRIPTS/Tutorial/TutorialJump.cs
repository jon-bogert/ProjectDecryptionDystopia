using UnityEngine;

public class TutorialJump : MonoBehaviour
{
    Animator _animator;
    [SerializeField] float _jumpRate = 2f;

    float _timer = 0f;

    bool _isJumping = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _timer = _jumpRate;
    }

    private void Update()
    {
        if (_timer <= 0)
        {
            _timer = _jumpRate;
            _isJumping = !_isJumping;
            _animator.SetBool("IsJumping", _isJumping);
        }
        _timer -= Time.deltaTime;
    }
}
