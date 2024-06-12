using System;
using UnityEngine;
using XephTools;

public class OccludeCamera : MonoBehaviour
{
    [SerializeField] float _fadeTime = 2f;
    Color _startColor;
    Color _endColor = Color.clear;

    Material _material;

    public Action blockEnd;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        _startColor = _material.color;


        //TODO - Remove 
        //GetComponent<MeshRenderer>().enabled = false;
    }

    public void Block()
    {
        ColorLerp lerp = new ColorLerp(_endColor, _startColor, _fadeTime, (val) => { _material.color = val; });
        lerp.OnComplete(() => blockEnd?.Invoke());
        OverTime.Add(lerp);
    }

    public void UnBlock()
    {
        ColorLerp lerp = new ColorLerp(_startColor, _endColor, _fadeTime, (val) => { _material.color = val; });
        OverTime.Add(lerp);
    }
}
