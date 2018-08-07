using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Timer {

    public bool isLooped { get; set; }
    public bool isPaused { get; set; }

    public float duration { get; private set; }
    public bool isCancelled { get; private set; }
    public bool isCompleted { get; private set; }

    private float accumulatedTime;

    private Action onComplete;
    private bool usesUnscaledTime;
    private bool hasAutoDestroyOwner;
    private UnityEngine.Object autoDestroyOwner;

    private Timer(float duration, Action onComplete, bool isLooped, bool useUnscaledTime) {
        this.duration = duration;
        this.onComplete = onComplete;

        this.isLooped = isLooped;
        this.isCancelled = false;
        this.usesUnscaledTime = useUnscaledTime;

        this.accumulatedTime = 0;
    }

    private float GetDeltaTime() {
        if (this.usesUnscaledTime) {
            return Time.unscaledDeltaTime;
        } else {
            return Time.deltaTime;
        }
    }

    public void SetAutoDestroyOwner(UnityEngine.Object owner) {
        this.autoDestroyOwner = owner;
        this.hasAutoDestroyOwner = owner != null;
    }

    public bool IsDone() {
        return this.isCompleted || this.isCancelled || (this.hasAutoDestroyOwner && this.autoDestroyOwner == null);
    }

    public void Cancel() {
        this.isCancelled = true;
    }

    public void Update() {
        if (this.IsDone() || this.isPaused) {
            return;
        }

        this.accumulatedTime += this.GetDeltaTime();
        if (this.accumulatedTime >= this.duration) {
            this.onComplete();

            if (this.isLooped) {
                this.accumulatedTime = 0;
            } else {
                this.isCompleted = true;
            }
        }
    }

    public float GetTimeRemaining() {
        return this.IsDone() ? 0 : this.duration - this.accumulatedTime;
    }

    public float GetPercentageComplete() {
        if (this.isCompleted) {
            return 1;
        }
        return this.accumulatedTime / this.duration;
    }

    public static Timer Register(float duration, Action onComplete, bool isLooped = false, bool useUnscaledTime = false, UnityEngine.Object autoCancelObj = null) {
        Timer timer = new Timer(duration, onComplete, isLooped, useUnscaledTime);
        timer.SetAutoDestroyOwner(autoCancelObj);
        if (TimerServiceLocator.instance != null && TimerServiceLocator.instance.timerManager != null) {
            TimerServiceLocator.instance.timerManager.AddTimer(timer);
        }
        return timer;
    }

    public static void Cancel(Timer timer) {
        if (timer != null) {
            timer.Cancel();
        }
    }

    public static void FinishImmediately(Timer timer) {
        if (timer == null) {
            return;
        }
        if (timer.IsDone()) {
            return;
        }
        timer.onComplete();
        timer.isCompleted = true;
    }
}