using UnityEngine;
using System.Collections;

public class SlowMoController : Singleton<SlowMoController> {

    private float slowMoSpeed;
    private float slowMoDuration;
    private float slowMoChangeSpeed;

    private float regularFixedDeltaTime;
    private float prevRealTime;

    void Start() {
        this.regularFixedDeltaTime = Time.fixedDeltaTime;
        this.prevRealTime = Time.realtimeSinceStartup;
    }

    void Update() {
        float deltaRealTime = Time.realtimeSinceStartup - this.prevRealTime;
        if (this.slowMoDuration > 0) {
            this.slowMoDuration = Mathf.MoveTowards(this.slowMoDuration, 0, deltaRealTime);
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, this.slowMoSpeed, this.slowMoChangeSpeed * deltaRealTime);
            Time.fixedDeltaTime = Mathf.Lerp(0, this.regularFixedDeltaTime, Time.timeScale);
        } else if (Time.timeScale < 1) {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1, this.slowMoChangeSpeed * deltaRealTime);
            Time.fixedDeltaTime = Mathf.Lerp(0, this.regularFixedDeltaTime, Time.timeScale);
        }
        this.prevRealTime = Time.realtimeSinceStartup;
    }

    public void SlowMo(float slowSpeed, float duration, float changeSpeed = 1000f) {
        this.slowMoSpeed = slowSpeed;
        this.slowMoDuration = duration;
        this.slowMoChangeSpeed = changeSpeed;
    }
}
