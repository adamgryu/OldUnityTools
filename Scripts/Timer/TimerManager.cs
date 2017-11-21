using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provide scene-local access to the TimerManager using a service locator.
/// </summary>
public class TimerServiceLocator : ServiceLocator<TimerServiceLocator> {

    public TimerManager timerManager { get { return LocateOrCreateServiceInActiveScene<TimerManager>(); } }

    public class TimerManager : ServiceMonoBehaviour {

        private List<Timer> timers = new List<Timer>();
        private List<Timer> timersToAddBuffer = new List<Timer>();

        private void Update() {
            UpdateAllRegisteredTimers();
        }

        private void OnDestroy() {
            CancelAllRegisteredTimers();
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
}