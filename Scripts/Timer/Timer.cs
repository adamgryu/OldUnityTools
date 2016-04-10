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

    private Action onComplete;
    private float startTime;
    private bool usesRealTime;
    private bool hasAutoDestroyOwner;
    private MonoBehaviour autoDestroyOwner;

    private static List<Timer> timers = new List<Timer>();
    private static List<Timer> timersToAddBuffer = new List<Timer>();
    private static TimerController sceneTimerController = null;

    private Timer(float duration, Action onComplete, bool isLooped, bool usesRealTime) {
        this.duration = duration;
        this.onComplete = onComplete;

        this.isLooped = isLooped;
        this.isCancelled = false;
        this.usesRealTime = usesRealTime;

        this.startTime = this.GetTime();
    }

    private float GetTime() {
        return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
    }

    private float GetDeltaTime() {
        if (this.usesRealTime) {
            Debug.LogWarning("Delta time is NOT supported for realtime timers!");
            return Time.deltaTime;
        } else {
            return Time.deltaTime;
        }
    }

    private float GetFireTime() {
        return this.startTime + this.duration;
    }

    public bool IsDone() {
        return this.isCompleted || this.isCancelled || (this.hasAutoDestroyOwner && this.autoDestroyOwner == null);
    }

    public void Cancel() {
        this.isCancelled = true;
    }

    public void Update() {
        if (this.isPaused) {
            this.startTime += this.GetDeltaTime();
            return;
        }
        if (this.IsDone()) {
            return;
        }

        if (this.GetTime() >= this.GetFireTime()) {
            this.onComplete();

            if (this.isLooped) {
                this.startTime = this.GetTime();
            } else {
                this.isCompleted = true;
            }
        }
    }

    public float GetPercentageComplete() {
        if (this.isCompleted) {
            return 1;
        }
        return (this.GetTime() - this.startTime) / this.duration;
    }

    public static Timer Register(float duration, Action onComplete, bool isLooped = false, bool useRealTime = false, MonoBehaviour autoCancelObj = null) {
        if (sceneTimerController == null && !TimerController.applicationIsQuitting) {
            GameObject obj = new GameObject("TimerController");
            sceneTimerController = obj.AddComponent<TimerController>();
        }

        Timer timer = new Timer(duration, onComplete, isLooped, useRealTime);
        Timer.timersToAddBuffer.Add(timer);
        timer.autoDestroyOwner = autoCancelObj;
        if (autoCancelObj != null) {
            timer.hasAutoDestroyOwner = true;
        }

        return timer;
    }

    public static void UpdateAllRegisteredTimers() {
        Timer.timers.AddRange(Timer.timersToAddBuffer);
        if (Timer.timersToAddBuffer.Count > 0) {
            Timer.timersToAddBuffer = new List<Timer>();
        }

        bool anyDone = false;
        foreach (Timer timer in Timer.timers) {
            timer.Update();
            anyDone |= timer.IsDone();
        }

        if (anyDone) {
            Timer.timers = Timer.timers.Where((t) => !(t.IsDone())).ToList(); // TODO: Optimize?
        }
    }

    public static void CancelAllRegisteredTimers() {
        foreach (Timer timer in Timer.timers) {
            timer.Cancel();
        }

        Timer.timers = new List<Timer>();
    }
}