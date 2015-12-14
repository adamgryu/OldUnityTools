using UnityEngine;
using System.Collections;

/// <summary>
/// Global Singletons are created when needed.
/// They are never present in a scene, and will be created as needed.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _instance;

    private static object _lock = new object();

    public static T instance {
        get {
            if (applicationIsQuitting) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (_lock) {
                if (_instance == null) {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1) {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if (_instance == null) {
                        // Check if the ResourceSingletonAttribute is present...
                        ResourceSingletonAttribute resource = null;
                        object[] attributes = typeof(T).GetCustomAttributes(false);
                        foreach (System.Attribute attribute in attributes) {
                            resource = attribute as ResourceSingletonAttribute;
                            if (resource != null) {
                                break;
                            }
                        }

                        if (resource != null) {
                            // Load a prefab that contains this singleton.
                            GameObject singleton = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(resource.resourceFilePath));
                            singleton.name = "(resource global singleton)" + singleton.name;
                            _instance = singleton.GetComponent<T>();
                            if (_instance == null) {
                                Debug.LogWarning("A prefab was loaded for the singleton, but the component was not on it!");
                            } else {
                                DontDestroyOnLoad(singleton);
                                Debug.Log("[Singleton] An instance of " + typeof(T) +
                                    " is needed in the scene, so '" + singleton +
                                    "' was loaded as a prefab with DontDestroyOnLoad.");
                            }
                        } else {
                            // Create a singleton component in the world.
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(global singleton) " + typeof(T).ToString();
                            DontDestroyOnLoad(singleton);
                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                    } else {
                        Debug.Log("[Singleton] Using instance already created: " +
                            _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy() {
        applicationIsQuitting = true;
    }
}