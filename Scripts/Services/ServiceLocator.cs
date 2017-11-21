using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides controlled access to services that exist locally within a scene. Concrete classes should
/// provide getters for various services and use the Locate() method to find and cache the service
/// reference. The cache is cleared when the active scene changes to point to the services in the new
/// scene.
/// </summary>
/// <typeparam name="T">The concrete type that is used for singleton access.</typeparam>
public abstract class ServiceLocator<T> : Singleton<T> where T : MonoBehaviour {

    protected Dictionary<Type, ServiceMonoBehaviour> cachedSceneServices = new Dictionary<Type, ServiceMonoBehaviour>();

    private void Awake() {
        SceneManager.activeSceneChanged += OnActiveSceneChange;
    }

    private void OnActiveSceneChange(Scene oldScene, Scene newScene) {
        cachedSceneServices.Clear();
        Log("Clearing the cached scene services.");
    }

    /// <summary>
    /// Looks for a service that exists as a MonoBehaviour in the current scene.
    /// </summary>
    protected T LocateServiceInActiveScene<T>() where T : ServiceMonoBehaviour {
        // Try and find a cached version of the service.
        Type serviceType = typeof(T);
        if (cachedSceneServices.ContainsKey(serviceType)) {
            return (T)cachedSceneServices[serviceType];
        }

        T foundService = SceneManager.GetActiveScene().FindComponentsOfTypeInScene<T>().FirstOrDefault();
        if (foundService == null) {
            LogError(serviceType, "Service is not present in this scene!");
        }

        Log("Located service and caching it.", serviceType);
        cachedSceneServices[serviceType] = foundService;
        return foundService;
    }

    private void Log(string message, Type service = null) {
        string serviceText = service != null ? service.Name + ": " : "";
        Debug.Log("[ServiceLocator] " + serviceText + message);
    }

    private void LogError(Type service, string warning) {
        string serviceText = service != null ? service.Name + ": " : "";
        Debug.LogError("[ServiceLocator] " + serviceText + warning);
    }
}