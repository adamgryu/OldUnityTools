using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour {

    public static bool applicationIsQuitting { get; private set; }

	private void Awake() {
		if (GameObject.FindObjectsOfType<TimerController>().Length > 1) {
			Debug.LogError("There are multiple TimerControllers in this scene!");
			GameObject.Destroy(this.gameObject);
		}
	}

    private void Update() {
		Timer.UpdateAllRegisteredTimers();
	}

    private void OnDestroy() {
		Timer.CancelAllRegisteredTimers();
	}

    private void OnApplicationQuit() {
        applicationIsQuitting = true;
    }
}