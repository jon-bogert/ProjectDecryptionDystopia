using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(DoorMovement))]
public class DoorLock : MonoBehaviour
{
    [SerializeField] TMP_Text _lockText;

    List<DoorKey> _keys = new();
    DoorMovement _movement;

    private void Awake()
    {
        _movement = GetComponent<DoorMovement>();
    }

    public void AddKey(DoorKey key)
    {
        _keys.Add(key);
        UpdateText();
    }

    public void RemoveKey(DoorKey key)
    {
        if (!_keys.Contains(key) )
        {
            Debug.LogError("Key was not found in collection");
            return;
        }

        _keys.Remove(key);
        UpdateText();
        if (_keys.Count <= 0)
        {
            _movement.OpenDoor();
        }
    }

    private void UpdateText()
    {
        _lockText.text = _keys.Count.ToString();
    }
}
