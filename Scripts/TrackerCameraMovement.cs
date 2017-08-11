using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TrackerCameraMovement : MonoBehaviour {

    protected static List<GameObject> EMPTY_LIST = new List<GameObject>();

    // Inspector Fields
    public float minDistanceBetweenPlayers = 3f;
    public float maxDistanceBetweenPlayers = 20f;

    public float minCameraSize = 5;
    public float maxCameraSize = 10;

    public float minDistanceFromWorld = 20;
    public float maxDistanceFromWorld = 50;

    public float cameraDriftLerp = 1f;
    public float acceleration = 12;

    public Vector3 cameraOffset;
    public List<GameObject> additionalCameraTargets;

    // Public Properties
    public Func<Vector3, Vector3> limitCameraMovement;
    public float speed { get; set; }

    // Private / Protected Properties
    protected Camera myCamera;
    protected Transform lockedTransform;
    protected bool isLocked;


    protected virtual void Awake() {
        this.myCamera = this.GetComponent<Camera>();
    }

    protected virtual void FixedUpdate() {
        float desiredSize = this.myCamera.orthographicSize;
        Vector3 desired = this.transform.position;

        // Find the desired position for the camera...
        if (this.GetGameObjectsToTrack().Count() > 0 || this.additionalCameraTargets.Count > 0) {
            desired = this.CalculatePlayerFocusCameraSettings(out desiredSize);
        }
        if (this.limitCameraMovement != null) {
            desired = this.limitCameraMovement(desired);
        }
        if (this.isLocked) {
            desired = this.lockedTransform.position;
        }

        // Clamp the desired movement by the speed and handle acceleration.
        Vector3 desiredMovement = (desired - this.transform.position) * cameraDriftLerp;
        if (desiredMovement.sqrMagnitude > this.speed.Sqr()) {
            float length = desiredMovement.magnitude;
            speed = Mathf.MoveTowards(this.speed, length, this.acceleration * Time.fixedDeltaTime);
            desiredMovement = desiredMovement * speed / length;
        } else {
            speed = desiredMovement.magnitude;
        }

        // Change the camera movement.
        this.transform.position += desiredMovement * Time.fixedDeltaTime;
        this.myCamera.orthographicSize += (desiredSize - this.myCamera.orthographicSize) * this.cameraDriftLerp * Time.fixedDeltaTime;
    }

    public virtual IEnumerable<GameObject> GetGameObjectsToTrack() {
        return EMPTY_LIST;
    }

    public Vector3 CalculatePlayerFocusCameraSettings(out float orthoSize) {
        IEnumerable<GameObject> ps = this.GetGameObjectsToTrack().Concat(this.additionalCameraTargets);
        if (ps.Count() == 0) {
            orthoSize = this.GetComponent<Camera>().orthographicSize;
            return this.transform.position;
        }

        Vector3 min = new Vector3(ps.Min(p => p.transform.position.x),
                                  ps.Min(p => p.transform.position.y),
                                  ps.Min(p => p.transform.position.z));
        Vector3 max = new Vector3(ps.Max(p => p.transform.position.x),
                                  ps.Max(p => p.transform.position.y),
                                  ps.Max(p => p.transform.position.z));

        float dist = (min - max).magnitude;
        float percentDistance = Mathf.InverseLerp(minDistanceBetweenPlayers, maxDistanceBetweenPlayers, dist);

        float useDist = Mathf.Lerp(minDistanceFromWorld, maxDistanceFromWorld, percentDistance);
        orthoSize = Mathf.Lerp(minCameraSize, maxCameraSize, percentDistance);

        Vector3 center = (min + max) / 2;
        return center - this.transform.forward * useDist + this.cameraOffset;
    }

    public virtual void LockTransform(Transform lockedTransform, bool snap = false) {
        this.isLocked = true;
        this.lockedTransform = lockedTransform;
        if (snap) {
            this.transform.position = lockedTransform.transform.position;
            this.transform.rotation = lockedTransform.transform.rotation;
        }
    }

    public virtual void UnlockTransform() {
        this.isLocked = false;
    }
}
