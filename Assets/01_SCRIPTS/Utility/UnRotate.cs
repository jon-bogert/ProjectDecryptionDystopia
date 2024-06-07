using UnityEngine;

public class UnRotate : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
