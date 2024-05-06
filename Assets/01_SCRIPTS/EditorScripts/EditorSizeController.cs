using TMPro;
using UnityEngine;

public class EditorSizeController : MonoBehaviour
{
    Vector3Int _size = Vector3Int.one;
    [SerializeField] TMP_Text xText;
    [SerializeField] TMP_Text yText;
    [SerializeField] TMP_Text zText;

    [SerializeField] EditorLevelSpace _levelSpace;

    private void Start()
    {
        UpdX(); UpdY(); UpdZ();
    }

    public void IncrX()
    {
        _size.x += 1;
        UpdX();
    }

    public void DecrX()
    {
        if (_size.x <= 1)
            return;

        _size.x -= 1;
        UpdX();
    }

    void UpdX()
    {
        xText.text = _size.x.ToString();
    }

    public void IncrY()
    {
        _size.y += 1;
        UpdY();
    }

    public void DecrY()
    {
        if (_size.y <= 1)
            return;

        _size.y -= 1;
        UpdY();
    }

    void UpdY()
    {
        yText.text = _size.y.ToString();
    }

    public void IncrZ()
    {
        _size.z += 1;
        UpdZ();
    }

    public void DecrZ()
    {
        if (_size.z <= 1)
            return;

        _size.z -= 1;
        UpdZ();
    }

    void UpdZ()
    {
        zText.text = _size.z.ToString();
    }

    public void CreateNew()
    {
        TileMap3D tileMap = new TileMap3D();
        tileMap.CreateEmpty(_size);

        _levelSpace.SetTilemap(tileMap);
    }
}
