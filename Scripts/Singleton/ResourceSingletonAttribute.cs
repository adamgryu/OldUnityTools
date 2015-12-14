using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Add this attribute to a Singleton class and it will load the Singleton from the Resources folder.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ResourceSingletonAttribute : Attribute {
    public readonly string resourceFilePath;

    public ResourceSingletonAttribute(string path) {
        this.resourceFilePath = path;
    }
}