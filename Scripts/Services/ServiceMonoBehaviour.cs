using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Classes that inherit from this can be found globally using the ServiceLocator class.
/// This behaviour also ensures that there is only one service per scene.
/// </summary>
public abstract class ServiceMonoBehaviour : MonoBehaviour {

    protected virtual void OnEnable() {
        var sceneObjects = gameObject.scene.FindComponentsOfTypeInScene(GetType());

        if (sceneObjects.AtLeast(2)) {
            Debug.LogError("[ServiceMonoBehaviour] " + GetType().Name + ": Multiple instances of this service in this scene.");
        }
    }
}