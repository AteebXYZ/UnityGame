using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdate
{
    public int Tick { get; private set; }
    public Vector3 Position { get; private set; }

    public TransformUpdate(int tick, Vector3 vector3)
    {
        Tick = tick;
        Position = vector3;
    }
}
