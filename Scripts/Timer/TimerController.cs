using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour {

	void Awake() {
		if (GameObject.FindObjectsOfType<TimerController>().Length > 1) {
			Debug.LogError("There are multiple TimerControllers in this scene!");
			GameObject.Destroy(this.gameObject);
		}
	}

	void Update() {
		Timer.UpdateAllRegisteredTimers();
	}

	void OnDestroy() {
		Timer.CancelAllRegisteredTimers();
	}
}