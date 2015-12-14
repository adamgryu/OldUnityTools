using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public static class QuickUnityExtensions {

    public static Vector3 Round(this Vector3 vector) {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static Vector3 SetX(this Vector3 vector, float x) {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 SetY(this Vector3 vector, float y) {
        return new Vector3(vector.x, y, vector.z);
    }

    public static Vector3 SetZ(this Vector3 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector2 SetX(this Vector2 vector, float x) {
        return new Vector2(x, vector.y);
    }

    public static Vector2 SetY(this Vector2 vector, float y) {
        return new Vector2(vector.x, y);
    }

    public static GameObject CloneAt(this GameObject obj, Vector3 position) {
        GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
        obj2.transform.position = position;
        return obj2;
    }

    public static GameObject Clone(this GameObject obj) {
        GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
        return obj2;
    }

    public static T MinValue<T>(this IEnumerable<T> collection, Func<T, float> heuristic) {
        T minObj = default(T);
        float min = float.PositiveInfinity;
        foreach (T t in collection) {
            float value = heuristic(t);
            if (value < min) {
                minObj = t;
                min = value;
            }
        }
        return minObj;
    }

    public static T PickRandom<T>(this IList<T> list) {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T PickRandom<T>(this IEnumerable<T> list) {
        int count = list.Count();
        if (count == 0) {
            return default(T);
        }
        return list.ElementAt(UnityEngine.Random.Range(0, count));
    }
}
