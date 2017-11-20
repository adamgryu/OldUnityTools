using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TimerController : MonoBehaviour {

    public static bool applicationIsQuitting { get; private set; }

    private List<Timer> timers = new List<Timer>();
    private List<Timer> timersToAddBuffer = new List<Timer>();

    private void OnEnable() {
        if (GameObject.FindObjectsOfType<TimerController>().Length > 1) {
            Debug.LogWarning("There are multiple TimerControllers in this scene!");
        }
    }

    private void Update() {
        UpdateAllRegisteredTimers();
    }

    private void OnDestroy() {
        CancelAllRegisteredTimers();
    }

    private void OnApplicationQuit() {
        applicationIsQuitting = true;
    }

    public void AddTimer(Timer timer) {
        timersToAddBuffer.Add(timer);
    }

    private void UpdateAllRegisteredTimers() {
        timers.AddRange(timersToAddBuffer);
        if (timersToAddBuffer.Count > 0) {
            timersToAddBuffer.Clear();
        }

        bool anyDone = false;
        foreach (Timer timer in timers) {
            timer.Update();
            anyDone |= timer.IsDone();
        }

        if (anyDone) {
            timers.RemoveAll(t => t.IsDone());
        }
    }

    private void CancelAllRegisteredTimers() {
        foreach (Timer timer in timers) {
            timer.Cancel();
        }

        timers.Clear();
    }
}