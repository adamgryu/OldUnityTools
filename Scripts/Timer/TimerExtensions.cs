using System;
using UnityEngine;

public static class TimerExtensions {
    public static Timer RegisterTimer(this MonoBehaviour owner, float duration, Action onComplete, bool isLooped, bool useRealTime) {
        return Timer.Register(duration, onComplete, isLooped, useRealTime, owner);
    }
}
