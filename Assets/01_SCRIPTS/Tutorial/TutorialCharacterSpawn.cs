using System.Collections.Generic;
using UnityEngine;

public class TutorialCharacterSpawn : MonoBehaviour
{
    [System.Serializable]
    public struct PrefabTransform
    {
        public Transform spawnPoint;
        public GameObject prefab;
    }

    [SerializeField] List<PrefabTransform> _characters;

    public void DoSpawn(Transform parent)
    {
        foreach (PrefabTransform character in _characters)
        {
            Instantiate(character.prefab, character.spawnPoint.position, character.spawnPoint.rotation, parent);
        }
        _characters.Clear();
    }
}
