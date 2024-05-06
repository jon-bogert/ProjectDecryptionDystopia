using UnityEngine;

public class EditorOffsetLevelSpace : MonoBehaviour
{
    [SerializeField] GameObject _cursor;
    [SerializeField] Transform _playerHand;

    EditorLevelSpace _parentSpace;

    private void Update()
    {
        if (!_parentSpace.hasValidTilemap)
            return;

        Vector3 handRelative = transform.InverseTransformPoint(_playerHand.transform.position);
        handRelative += Vector3.one * 0.5f;
        handRelative.x = Mathf.Round(handRelative.x);
        handRelative.y = Mathf.Round(handRelative.y);
        handRelative.z =Mathf.Round(handRelative.z);

        bool isOutBounds =
            (handRelative.x < 0 || handRelative.x >= _parentSpace.dimensions.x
            || handRelative.y < 0 || handRelative.y >= _parentSpace.dimensions.y
            || handRelative.z < 0 || handRelative.z >= _parentSpace.dimensions.z);

        if (isOutBounds && _cursor.activeSelf)
            _cursor.SetActive(false);
        else if (!isOutBounds && !_cursor.activeSelf)
            _cursor.SetActive(true);

        if (isOutBounds)
            return;

        _cursor.transform.position = transform.TransformPoint(handRelative);
    }

    public void SetParentSpace(EditorLevelSpace parentSpace)
    {
        _parentSpace = parentSpace;
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
