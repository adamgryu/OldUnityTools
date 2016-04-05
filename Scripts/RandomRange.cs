using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct RandomRange  {
    public float min;
    public float max;

    public float Random() {
        return UnityEngine.Random.Range(this.min, this.max);
    }
}