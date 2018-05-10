using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ExportableScene {
    [SerializeField]
    private UnityEngine.Object sceneReference; // These are SceneAsset objects that disappear in an actual build.

    [SerializeField]
    private string sceneName = "";

    /// <summary>
    /// Please call this in MonoBehaviours where this is used.
    /// </summary>
    public void Validate(MonoBehaviour dirtyTarget) {
        string name = sceneReference != null ? sceneReference.name : "";
        if (sceneName != name) {
            Debug.LogWarning("Click here to go to the object and fix outdated ExportableScene reference for " + dirtyTarget.gameObject.name, dirtyTarget);
        }
    }

    public string GetSceneName() {
        if (sceneReference != null) {
            if (sceneReference.name != sceneName) {
                Debug.LogError("An ExportableScene reference is out of date: " + sceneName + " -> " + sceneReference.name);
            }
            // NOTE: Cannot verify that the scene name is NULL when the object reference is null.
        }
        return sceneName;
    }
}
