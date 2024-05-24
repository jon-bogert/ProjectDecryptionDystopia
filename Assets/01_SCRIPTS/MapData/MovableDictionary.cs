using System.Collections.Generic;
using UnityEngine;

public class MovableDictionary : MonoBehaviour
{
    [System.Serializable]
    public struct MovablePair
    {
        public string key;
        public GameObject prefab;
    }

    private static MovableDictionary inst;

    [SerializeField] List<MovablePair> _playerMovables = new();
    [SerializeField] List<MovablePair> _selfMovables = new();

    Dictionary<string, GameObject> _playerMovablesHashed = new();
    Dictionary<string, GameObject> _selfMovablesHashed = new();

    void Awake()
    {
        Debug.Log("1");
        if (inst != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        inst = this;

        foreach(MovablePair pair in _playerMovables)
        {
            _playerMovablesHashed.Add(pair.key, pair.prefab);
        }
        foreach (MovablePair pair in _selfMovables)
        {
            _selfMovablesHashed.Add(pair.key, pair.prefab);
        }
    }

    public static GameObject GetPlayerMovable(string key)
    {
        return inst._playerMovablesHashed[key];
    }
    public static GameObject GetSelfMovable(string key)
    {
        Debug.Log("2");
        return inst._selfMovablesHashed[key];
    }
}
