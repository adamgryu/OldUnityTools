using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public static class QuickUnityExtensions {

    public static GameObject CloneAt(this GameObject obj, Vector3 position) {
        GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
        obj2.transform.position = position;
        return obj2;
    }

    public static GameObject Clone(this GameObject obj) {
        GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
        return obj2;
    }

    public static string ColorToHexString(this Color32 color) {
        string hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color SetA(this Color color, float a) {
        return new Color(color.r, color.g, color.b, a);
    }

    public static Vector3 WorldMousePosition(this Camera camera, float? distanceFromCamera = null) {
        if (distanceFromCamera == null) {
            distanceFromCamera = 0;
        }
        return camera.ScreenToWorldPoint(Input.mousePosition.SetZ(distanceFromCamera.Value));
    }

    #region Collections

    public static void BufferedForEach<T>(this IEnumerable<T> collection, Func<T, bool> condition, Action<T> performIf) {
        LinkedList<T> buffer = new LinkedList<T>();
        foreach (T obj in collection) {
            if (condition(obj)) {
                buffer.AddFirst(obj);
            }
        }
        foreach (T obj in buffer) {
            performIf(obj);
        }
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

    public static T MaxValue<T>(this IEnumerable<T> collection, Func<T, float> heuristic) {
        T minObj = default(T);
        float max = float.NegativeInfinity;
        foreach (T t in collection) {
            float value = heuristic(t);
            if (value > max) {
                minObj = t;
                max = value;
            }
        }
        return minObj;
    }

    public static int MinValueIndex<T>(this IEnumerable<T> collection, Func<T, float> heuristic) {
        int minIndex = 0;
        float min = float.PositiveInfinity;
        int index = 0;
        foreach (T t in collection) {
            float value = heuristic(t);
            if (value < min) {
                minIndex = index;
                min = value;
            }
            index++;
        }
        return minIndex;
    }

    public static IList<T> Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
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

    public static T PickRandomWithWeights<T>(this List<T> list, IList<float> weights) {
        float total = weights.Sum();
        float value = UnityEngine.Random.Range(0, total);

        int index = 0;
        float sum = 0;
        while (index < weights.Count && sum + weights[index] < value) {
            sum += weights[index];
            index++;
        }
        if (index >= weights.Count) {
            Debug.LogWarning("Something went wrong...");
            return list[0];
        }
        return list[index];
    }

    #endregion
}
