using UnityEngine;

/// <summary>
/// Represents a single-instance object that is placed in scenes.
/// This will not create the singleton, it just provides quick access to the Singleton.
/// 
/// I think this class is still kind of buggy in some situations, I'll fix it later.
/// </summary>
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;

    public static T instance {
        get {
            // HACK: Search for a new one if the current is disabled; this is needed for my current project.
            if (_instance != null && !_instance.isActiveAndEnabled) {
                _instance = null;
                Debug.Log("[SceneSingleton] The current instance of " + typeof(T) + " is inactive. Searching for an active one...");
            }

            if (_instance == null) {
                _instance = (T)GameObject.FindObjectOfType(typeof(T));

                if (GameObject.FindObjectsOfType(typeof(T)).Length > 1) {
                    Debug.LogError("[SceneSingleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                    return _instance;
                }

                if (_instance == null) {
                    Debug.LogWarning("[SceneSingleton] An instance of " + typeof(T) + " is needed in the scene, but none exists!");
                } else {
                    Debug.Log("[SceneSingleton] Using instance already created: " + _instance.gameObject.name);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake() {
        // Placeholder.
    }

    protected virtual void OnDestroy() {
        _instance = null;
    }
}