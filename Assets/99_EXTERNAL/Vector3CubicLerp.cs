using System;
using UnityEngine;
using XephTools;

public class Vector3CubicLerp : OverTime.ModuleBase
{
    private readonly Vector3 start;
    private readonly Vector3 end;
    private readonly Vector3 weightStart;
    private readonly Vector3 weightEnd;
    private readonly Action<Vector3> setter;

    public Vector3CubicLerp(Vector3 start, Vector3 end, Vector3 weightStart, Vector3 weightEnd, float length, Action<Vector3> setter)
    {
        this.start = start;
        this.end = end;
        this.weightStart = weightStart;
        this.weightEnd = weightEnd;
        this.setter = setter;
        Init(length);
    }

    internal override void Update()
    {
        Vector3 a = start + Progress * (weightStart - start);
        Vector3 b = weightStart + Progress * (weightEnd - weightStart);
        Vector3 c = weightEnd + Progress * (end - weightEnd);

        Vector3 d = a + Progress * (b - a);
        Vector3 e = b + Progress * (c - b);

        setter(d + Progress * (e - d));
    }
}
