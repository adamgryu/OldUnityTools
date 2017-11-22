using System;
using UnityEngine;

public static class TimerExtensions {
    public static Timer RegisterTimer(this MonoBehaviour owner, float duration, Action onComplete, bool isLooped = false, bool useUnscaledTime = false) {
        return Timer.Register(duration, onComplete, isLooped, useUnscaledTime, owner);
    }
}