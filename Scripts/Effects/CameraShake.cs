using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour {

    public static void ShakeAllCameras(float percent, float shakeMultiplier = 1) {
        foreach(Camera camera in Camera.allCameras) {
            CameraShake shake = camera.GetComponent<CameraShake>();
            if (shake != null) {
                shake.Shake(percent, shakeMultiplier);
            }
        }
    }

    const float SHAKE_DELTA = 0.01f;

    public float defaultShakeStrength;
    public float shakePercentDecreaseSpeed;
    public AnimationCurve shakeStrengthPercentCurve;

    public bool isShaking { get { return this.shakePercent > SHAKE_DELTA; } }

    private float shakePercent;
    private float shakeStrengthMultiplier;
    private Vector3 lastShakeVector;

    void Update() {
        if (this.isShaking) {
            this.shakePercent = Mathf.MoveTowards(this.shakePercent, 0, this.shakePercentDecreaseSpeed * Time.deltaTime);
        }
    }

    public void Shake(float percent, float shakeMultiplier = 1) {
        if (this.isShaking) {
            this.shakeStrengthMultiplier = Mathf.Max(this.shakeStrengthMultiplier, shakeMultiplier);
        } else {
            this.shakeStrengthMultiplier = shakeMultiplier;
        }
        this.shakePercent = Mathf.Max(this.shakePercent, percent);
    }

    void OnPreRender() {
        if (this.shakePercent > SHAKE_DELTA) {
            this.lastShakeVector = UnityEngine.Random.insideUnitSphere * this.shakeStrengthPercentCurve.Evaluate(this.shakePercent) * this.defaultShakeStrength * this.shakeStrengthMultiplier;
            this.transform.position += this.lastShakeVector;
        }
    }

    void OnPostRender() {
        if (this.shakePercent > SHAKE_DELTA) {
            this.transform.position -= this.lastShakeVector;
        }
    }
}
