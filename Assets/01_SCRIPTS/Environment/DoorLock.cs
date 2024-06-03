using UnityEngine;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(DoorMovement))]
public class DoorLock : MonoBehaviour
{
    [SerializeField] TMP_Text _lockText;

    List<DoorKey> _keys = new();
    DoorMovement _movement;

    SoundPlayer3D _soundPlayer;

    private void Awake()
    {
        _movement = GetComponent<DoorMovement>();
    }

    private void Start()
    {
        _soundPlayer = FindObjectOfType<SoundPlayer3D>();
        if (_soundPlayer == null)
            Debug.LogError("Couldn't find Sound Player in Scene");
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
            _soundPlayer.Play("door-open", transform.position, SoundPlayer3D.Bank.Single);
            _movement.OpenDoor();
        }
    }

    private void UpdateText()
    {
        _lockText.text = _keys.Count.ToString();
    }
}
