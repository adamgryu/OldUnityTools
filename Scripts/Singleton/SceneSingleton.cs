using UnityEngine;
using System.Collections;

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
            if (_instance == null) {
                _instance = (T)GameObject.FindObjectOfType(typeof(T));

                if (GameObject.FindObjectsOfType(typeof(T)).Length > 1) {
                    Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                    return _instance;
                }

                if (_instance == null) {
                    Debug.LogWarning("[Singleton] An instance of " + typeof(T) + " is needed in the scene, but none exists!");
                } else {
                    Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name);
                }
            }

            return _instance;
        }
    }

    protected virtual void OnDestroy() {
        _instance = null;
    }
}