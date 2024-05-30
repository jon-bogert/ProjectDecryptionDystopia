using XephTools;
using UnityEngine;
using System;

public class ColorLerp : OverTime.ModuleBase
{
    private readonly Color start;
    private readonly Color end;
    private readonly Action<Color> setter;

    public ColorLerp(Color start, Color end, float length, Action<Color> setter)
    {
        this.start = start;
        this.end = end;
        this.setter = setter;
        Init(length);
    }

    internal override void Update()
    {
        setter(Color.Lerp(start, end, Progress));
    }
}
