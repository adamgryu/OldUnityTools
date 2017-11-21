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
/// <typeparam name="TMyType">The concrete type that is used for singleton access.</typeparam>
public abstract class ServiceLocator<TMyType> : Singleton<TMyType> where TMyType : MonoBehaviour {

    private static bool applicationIsQuitting = false;

    protected Dictionary<Type, ServiceMonoBehaviour> cachedSceneServices = new Dictionary<Type, ServiceMonoBehaviour>();


    protected virtual void Awake() {
        SceneManager.activeSceneChanged += OnActiveSceneChange;
    }

    protected virtual void OnApplicationQuit() {
        applicationIsQuitting = true;
    }

    private void OnActiveSceneChange(Scene oldScene, Scene newScene) {
        cachedSceneServices.Clear();
        Log("Clearing the cached scene services.");
    }

    /// <summary>
    /// Looks for a service that exists as a MonoBehaviour in the current scene.
    /// </summary>
    protected TService LocateServiceInActiveScene<TService>() where TService : ServiceMonoBehaviour {
        var service = LocateServiceInActiveSceneWithoutErrors<TService>();
        if (service == null) {
            LogError(typeof(TService), "Service is not present in this scene!");
        }
        return service;
    }

    private TService LocateServiceInActiveSceneWithoutErrors<TService>() where TService : ServiceMonoBehaviour {
        // Try and find a cached version of the service.
        Type serviceType = typeof(TService);
        if (cachedSceneServices.ContainsKey(serviceType)) {
            return (TService)cachedSceneServices[serviceType];
        }

        TService foundService = SceneManager.GetActiveScene().FindComponentsOfTypeInScene<TService>().FirstOrDefault();
        if (foundService == null) {
            return null;
        }

        Log(serviceType, "Located service and caching it.");
        cachedSceneServices[serviceType] = foundService;
        return foundService;
    }

    protected TService LocateOrCreateServiceInActiveScene<TService>() where TService : ServiceMonoBehaviour {
        TService service = LocateServiceInActiveSceneWithoutErrors<TService>();
        if (service != null) {
            return service;
        }

        if (applicationIsQuitting) {
            return null;
        }

        Type serviceType = typeof(TService);
        Log(serviceType, "Created a new service in the scene.");
        GameObject obj = new GameObject("(Created Service) " + serviceType.Name);
        service = obj.AddComponent<TService>();
        cachedSceneServices[serviceType] = service;
        return service;
    }

    private void Log(string message) {
        Log(null, message);
    }

    private void Log(Type service, string message) {
        string serviceText = service != null ? service.Name + ": " : "";
        Debug.Log("[" + GetType().Name + "] " + serviceText + message);
    }

    private void LogError(Type service, string warning) {
        string serviceText = service != null ? service.Name + ": " : "";
        Debug.LogError("[" + GetType().Name + "] " + serviceText + warning);
    }
}