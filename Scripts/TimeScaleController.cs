using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeScaleController : Singleton<TimeScaleController> {

    private const float TIME_SCALE_CHANGE_SPEED = 1000;

    public float baseTimeScale { get; set; }

    private float slowMoScale;
    private float slowMoDuration;

    private float timeScaleChangeSpeed = TIME_SCALE_CHANGE_SPEED;
    private float initalFixedDeltaTime;

    private SortedList<StackResourceSortingKey, float> timeScaleAdjustments = new SortedList<StackResourceSortingKey, float>();
    private IList<float> timeScaleListCache;

    private void Start() {
        initalFixedDeltaTime = Time.fixedDeltaTime;
        timeScaleListCache = timeScaleAdjustments.Values;
        baseTimeScale = 1;
    }

    private void Update() {
        float realDeltaTime = Time.unscaledDeltaTime;

        if (slowMoDuration > 0) {
            // Apply the slow mo.
            slowMoDuration = Mathf.MoveTowards(slowMoDuration, 0, realDeltaTime);
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, slowMoScale, timeScaleChangeSpeed * realDeltaTime);
            Time.fixedDeltaTime = Mathf.LerpUnclamped(0, initalFixedDeltaTime, Time.timeScale);
        } else {
            // Return to normal speed.
            float desiredTimeScale = baseTimeScale;
            if (timeScaleAdjustments.Count > 0) {
                desiredTimeScale *= timeScaleListCache[0];
            }

            if (Time.timeScale != desiredTimeScale) {
                Time.timeScale = Mathf.MoveTowards(Time.timeScale, desiredTimeScale, timeScaleChangeSpeed * realDeltaTime);
                if (Time.timeScale == desiredTimeScale) {
                    // Once back to speed, return the the default change speed.
                    timeScaleChangeSpeed = TIME_SCALE_CHANGE_SPEED;
                }
            }

            // Update the fixed time to match the timescale.
            float fixedDeltaTime = Mathf.Min(initalFixedDeltaTime, Mathf.LerpUnclamped(0, initalFixedDeltaTime, Time.timeScale));
            if (Time.fixedDeltaTime != fixedDeltaTime) {
                Time.fixedDeltaTime = fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Adjusts the timescale. The returned key must be used to unregister the adjustment and return it to normal.
    /// </summary>
    public StackResourceSortingKey AdjustTimeScale(float timeScale) {
        var resourceKey = new StackResourceSortingKey(0, key => timeScaleAdjustments.Remove(key));
        timeScaleAdjustments.Add(resourceKey, timeScale);
        return resourceKey;
    }

    /// <summary>
    /// Slows down time for a some realtime seconds.
    /// </summary>
    public void SlowMo(float slowTimeScale, float duration, float? timeChangeSpeed) {
        slowMoScale = slowTimeScale;
        slowMoDuration = duration;
        if (timeChangeSpeed.HasValue) {
            timeScaleChangeSpeed = timeChangeSpeed.Value;
        }
    }
}
