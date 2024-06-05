using System;
using UnityEngine;

public class ButtonBounds : MonoBehaviour
{
    public Action<Collider> collisionEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled)
            return;

        collisionEvent?.Invoke(other);
    }
}
