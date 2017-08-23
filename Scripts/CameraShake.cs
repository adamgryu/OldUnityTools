using UnityEngine;
using System.Collections;

/// <summary>
/// Adds a shaking effect to the camera which can be triggered through code. A random vector is
/// added to the camera position on OnPreRender() if a shake is occuring.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour {

    const float SHAKE_DELTA = 0.01f;

    [Tooltip("The maximum distance the camera can move from its normal position while shaking.")]
    public float shakeDistance = 0.3f;

    [Tooltip("The time (in seconds) it takes for the camera to stop shaking.")]
    public float shakeCooldownTime = 1;

    [Tooltip("The strength of the camera shake over the course of its cooldown time.")]
    public AnimationCurve shakeCooldownCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public bool isShaking { get { return this.shakeCooldown > SHAKE_DELTA; } }

    private float shakeCooldown;
    private float shakeStrength;
    private Vector3 lastShakeVector;

    private float adjustedCooldownTime { get { return this.shakeCooldownTime * this.shakeStrength; } }

    /// <summary>
    /// Shakes the camera.
    /// </summary>
    /// <param name="shakeStrength">A multiplier applied to the default shake distance and shake cooldown time.</param>
    /// <param name="overrideShakeStrength">If true, the new shake strength will be applied even if it is smaller than the current shake.</param>
    public void Shake(float shakeStrength = 1, bool overrideShakeStrength = false) {
        this.shakeStrength = (overrideShakeStrength || !this.isShaking) ? shakeStrength : Mathf.Max(shakeStrength, this.shakeStrength);
        this.shakeCooldown = this.adjustedCooldownTime;
    }

    /// <summary>
    /// Shakes all cameras in the scene.
    /// </summary>
    public static void ShakeAllCameras(float shakeStrength = 1) {
        foreach (Camera camera in Camera.allCameras) {
            CameraShake shake = camera.GetComponent<CameraShake>();
            if (shake != null) {
                shake.Shake(shakeStrength);
            }
        }
    }

    private void Update() {
        if (this.isShaking) {
            this.shakeCooldown = Mathf.MoveTowards(this.shakeCooldown, 0, Time.deltaTime);
        }
    }

    private void OnPreRender() {
        if (this.shakeCooldown > SHAKE_DELTA) {
            float shakePercent = this.shakeCooldown / this.adjustedCooldownTime;
            this.lastShakeVector = Random.insideUnitSphere * this.shakeCooldownCurve.Evaluate(shakePercent) * this.shakeStrength * this.shakeDistance;
            this.transform.position += this.lastShakeVector;
        }
    }

    private void OnPostRender() {
        if (this.shakeCooldown > SHAKE_DELTA) {
            this.transform.position -= this.lastShakeVector;
        }
    }
}
