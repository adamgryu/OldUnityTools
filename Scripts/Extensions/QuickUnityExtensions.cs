using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using QuickUnityTools.Audio;

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

    public static GameObject CloneInactive(this GameObject obj) {
        obj.gameObject.SetActive(false);
        GameObject obj2 = GameObject.Instantiate<GameObject>(obj);
        obj.gameObject.SetActive(true);
        return obj2;
    }

    public static string ColorToHexString(this Color32 color) {
        string hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static Color HexStringToColor(string hex) {
        try {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        } catch (FormatException) {
            return Color.black;
        }
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

    public static Vector3 RandomWithin(this Bounds bounds) {
        Vector3 randomWithin = new Vector3(
            bounds.size.x * UnityEngine.Random.value,
            bounds.size.y * UnityEngine.Random.value,
            bounds.size.z * UnityEngine.Random.value);
        return bounds.min + randomWithin;
    }

    public static Rect SplitHorizontal(this Rect rect, int number, int index, float spacing = 5) {
        float width = (rect.width - (spacing * Math.Max(0, number - 1))) / number;
        return new Rect(rect.x + (width + spacing) * index, rect.y, width, rect.height);
    }

    public static Rect SplitHorizontal(this Rect rect, float splitPercent, bool first, float spacing = 5) {
        return new Rect(
            rect.x + (first ? 0 : (rect.width - spacing) * splitPercent + spacing),
            rect.y,
            (rect.width - spacing) * (first ? splitPercent : 1 - splitPercent),
            rect.height);
    }

    public static Rect SplitVertical(this Rect rect, int number, int index, float spacing = 5) {
        float height = (rect.height - (spacing * Math.Max(0, number - 1))) / number;
        return new Rect(rect.x, rect.y + (height + spacing) * index, rect.width, height);
    }

    public static string CamelCaseToSpaces(this string text) {
        return Regex.Replace(text, "(\\B[A-Z])", " $1");
    }

    public static IEnumerable<Transform> GetChildren(this Transform transform) {
        foreach (Transform child in transform) {
            yield return child;
        }
    }

    public static void DestroyChildren(this Transform transform) {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static Vector3 TransformPointTo(this RectTransform from, Vector3 point, RectTransform to) {
        return to.InverseTransformPoint(from.TransformPoint(point));
    }

    public static Vector2 WorldToRectTransform(Vector3 worldPosition, RectTransform toTransform) {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);
        RectTransform canvasRect = toTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector2 canvasPoint = Vector2.Scale(canvasRect.sizeDelta, (viewportPoint - Vector2.one * 0.5f));
        return TransformPointTo(canvasRect, canvasPoint, toTransform);
    }

    public static GameObject GetGameObject(this ResourceRequest request) {
        return request.asset as GameObject;
    }

    public static IEnumerable<T> FindComponentsOfTypeInScene<T>(this Scene scene) where T : Component {
        foreach (GameObject rootObj in scene.GetRootGameObjects()) {
            foreach (T foundObj in rootObj.GetComponentsInChildren<T>()) {
                yield return foundObj;
            }
        }
    }

    public static IEnumerable<Component> FindComponentsOfTypeInScene(this Scene scene, Type type) {
        foreach (GameObject rootObj in scene.GetRootGameObjects()) {
            foreach (Component foundObj in rootObj.GetComponentsInChildren(type)) {
                yield return foundObj;
            }
        }
    }

    public static bool IsPointWithin(this Collider collider, Vector3 point) {
        if (!collider.bounds.Contains(point)) {
            return false;
        }

        Vector3 towardCenter = collider.bounds.center - point;
        RaycastHit hit;
        return !collider.Raycast(new Ray(point, towardCenter), out hit, towardCenter.magnitude);
    }

    public static IEnumerable<T> Yield<T>(this T item) {
        yield return item;
    }

    public static AudioSource Play(this AudioClip clip) {
        return SoundPlayer.instance.Play(clip);
    }

    public static bool IncludesLayer(this LayerMask mask, int layer) {
        return mask == (mask | (1 << layer));
    }

    public static IEnumerator WrapAsEnumerator(this YieldInstruction yieldInstruction) {
        yield return yieldInstruction;
    }

    #region Collections

    public static void IndexedForEach<T>(this IEnumerable<T> collection, Action<T, int> action) {
        int i = 0;
        foreach (T element in collection) {
            action(element, i++);
        }
    }

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

    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        int index = 0;
        foreach (var item in source) {
            if (predicate(item)) {
                return index;
            }
            index++;
        }
        return -1;
    }

    public static int LastIndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
        int reverseIndex = source.Reverse().IndexOf(predicate);
        if (reverseIndex == -1) {
            return -1;
        }
        return source.Count() - 1 - reverseIndex;
    }

    public static bool AtLeast<T>(this IEnumerable<T> collection, int count, Func<T, bool> predicate = null) {
        if (predicate == null) {
            predicate = item => true;
        }

        int itemsSeen = 0;
        foreach (T item in collection) {
            if (predicate(item)) {
                itemsSeen++;
                if (itemsSeen == count) {
                    return true;
                }
            }
        }
        return false;
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

    public static void Resize<T>(this List<T> list, int size, T element = default(T)) {
        int count = list.Count;

        if (size < count) {
            list.RemoveRange(size, count - size);
        } else if (size > count) {
            if (size > list.Capacity) {
                list.Capacity = size; // Optimization.
            }
            list.AddRange(Enumerable.Repeat(element, size - count));
        }
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

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    #endregion
}
