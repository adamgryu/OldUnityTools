using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct RandomRange  {
    public float min;
    public float max;

    public RandomRange(float min, float max) {
        this.max = max;
        this.min = min;
    }

    public float Random() {
        return UnityEngine.Random.Range(this.min, this.max);
    }
}