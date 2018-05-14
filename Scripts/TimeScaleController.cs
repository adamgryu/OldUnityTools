using UnityEngine;
using System.Collections;

public class TimeScaleController : Singleton<TimeScaleController> {

    private const float TIME_SCALE_CHANGE_SPEED = 1000;

    public float normalTimeScale = 1;

    private float slowMoScale;
    private float slowMoDuration;

    private float timeScaleChangeSpeed = TIME_SCALE_CHANGE_SPEED;
    private float initalFixedDeltaTime;

    public float secretSpeedUp = 1; // HACK

    void Start() {
        initalFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update() {
        float realDeltaTime = Time.unscaledDeltaTime;

        if (slowMoDuration > 0) {
            // Apply the slow mo.
            slowMoDuration = Mathf.MoveTowards(slowMoDuration, 0, realDeltaTime);
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, slowMoScale, timeScaleChangeSpeed * realDeltaTime);
            Time.fixedDeltaTime = Mathf.LerpUnclamped(0, initalFixedDeltaTime, Time.timeScale);
        } else {
            // Return to normal speed.
            float hack = secretSpeedUp * normalTimeScale;
            if (Time.timeScale != hack) {
                Time.timeScale = Mathf.MoveTowards(Time.timeScale, hack, timeScaleChangeSpeed * realDeltaTime);
                if (Time.timeScale == hack) {
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

    public void SlowMo(float slowTimeScale, float duration, float? timeChangeSpeed) {
        slowMoScale = slowTimeScale;
        slowMoDuration = duration;
        if (timeChangeSpeed.HasValue) {
            timeScaleChangeSpeed = timeChangeSpeed.Value;
        }
    }
}
