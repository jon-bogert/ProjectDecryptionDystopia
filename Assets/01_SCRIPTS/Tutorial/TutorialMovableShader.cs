using UnityEngine;
using UnityEngine.Events;

public class TutorialMovableShader : MonoBehaviour
{
    [SerializeField] UnityEvent shaderSelect;
    [SerializeField] UnityEvent shaderDeselect;

    public void ShaderSelect()
    {
        shaderSelect?.Invoke();
    }
    public void ShaderDeselect()
    {
        shaderDeselect?.Invoke();
    }
}
