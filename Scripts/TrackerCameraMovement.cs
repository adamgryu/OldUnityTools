using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TrackerCameraMovement : MonoBehaviour {

    protected static List<GameObject> EMPTY_LIST = new List<GameObject>();

	public float minDistanceBetweenPlayers = 3f;
	public float maxDistanceBetweenPlayers = 20f;

	public float minCameraSize = 5;
    public float maxCameraSize = 10;

    public float minDistanceFromWorld = 20;
    public float maxDistanceFromWorld = 50;

	public float cameraDriftLerp = 1f;

    public Vector3 cameraOffset;

	public List<GameObject> additionalCameraTargets;
    public Func<Vector3, Vector3> limitCameraMovement;


    public virtual IEnumerable<GameObject> GetGameObjectsToTrack() {
        return EMPTY_LIST;
    }

	void FixedUpdate() {
		if (this.GetGameObjectsToTrack().Count() > 0 || this.additionalCameraTargets.Count > 0) {
			float desiredSize;
			Vector3 desired = this.CalculatePlayerFocusCameraSettings(out desiredSize);
            if (this.limitCameraMovement != null) {
                desired = this.limitCameraMovement(desired);
            }
			this.transform.position += (desired - this.transform.position) * cameraDriftLerp * Time.fixedDeltaTime;
			Camera.main.orthographicSize += (desiredSize - Camera.main.orthographicSize) * cameraDriftLerp * Time.fixedDeltaTime;
		}
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
}
