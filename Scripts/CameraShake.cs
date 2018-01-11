using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Adds a shaking effect to the camera which can be triggered through code. A random vector is
/// added to the camera position on OnPreRender() if a shake is occuring.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour {

    [Serializable]
    public class ShakeConfiguration {
        [Tooltip("The maximum distance the camera can move from its normal position while shaking.")]
        public float maxShakeDistance = 0.3f;

        [Tooltip("The time (in seconds) it takes for the camera to stop shaking.")]
        public float shakeTime = 1;

        [Tooltip("The strength of the camera shake over the course of its cooldown time.")]
        public AnimationCurve shakeDistanceOverTime = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    public enum Intensity {
        Tiny,
        Gentle,
        Moderate,
        Heavy,
        Extreme,
    }

    public ShakeConfiguration tinyShake;
    public ShakeConfiguration gentleShake;
    public ShakeConfiguration moderateShake;
    public ShakeConfiguration heavyShake;
    public ShakeConfiguration extremeShake;

    public bool isShaking { get { return this.shakeCooldown > 0.05f; } }

    private Dictionary<Intensity, ShakeConfiguration> shakeConfigs;
    private Intensity lastIntensity;
    private ShakeConfiguration shakeConfig;

    private float shakeCooldown;
    private Vector3 lastShakeVector;

    private void Awake() {
        shakeConfigs = new Dictionary<Intensity, ShakeConfiguration>();
        shakeConfigs[Intensity.Tiny] = tinyShake;
        shakeConfigs[Intensity.Gentle] = gentleShake;
        shakeConfigs[Intensity.Moderate] = moderateShake;
        shakeConfigs[Intensity.Heavy] = heavyShake;
        shakeConfigs[Intensity.Extreme] = extremeShake;
    }

    /// <summary>
    /// Shakes the camera.
    /// </summary>
    public void Shake(Intensity intensity) {
        if (!isShaking || intensity > lastIntensity) {
            lastIntensity = intensity;
            shakeConfig = shakeConfigs[intensity];
            shakeCooldown = shakeConfig.shakeTime;
        }
    }

    /// <summary>
    /// Shakes all cameras in the scene.
    /// </summary>
    public static void ShakeAllCameras(Intensity intensity) {
        foreach (Camera camera in Camera.allCameras) {
            CameraShake shake = camera.GetComponent<CameraShake>();
            if (shake != null) {
                shake.Shake(intensity);
            }
        }
    }

    private void Update() {
        if (isShaking) {
            shakeCooldown = Mathf.MoveTowards(shakeCooldown, 0, Time.unscaledDeltaTime);
        }

        // Debug shakes by holding R, T, Y, and then pressing a number from 1 to 5.
        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.Y)) {
            for (KeyCode key = KeyCode.Alpha1; key <= KeyCode.Alpha5; key++) {
                if (Input.GetKeyDown(key)) {
                    Shake((Intensity)(int)(key - KeyCode.Alpha1));
                }
            }
        }
    }

    private void OnPreRender() {
        if (isShaking) {
            float shakePercent = shakeCooldown / shakeConfig.shakeTime;
            this.lastShakeVector = UnityEngine.Random.insideUnitSphere
                * shakeConfig.shakeDistanceOverTime.Evaluate(shakePercent)
                * shakeConfig.maxShakeDistance;
            this.transform.position += this.lastShakeVector;
        }
    }

    private void OnPostRender() {
        if (isShaking) {
            this.transform.position -= this.lastShakeVector;
        }
    }
}
