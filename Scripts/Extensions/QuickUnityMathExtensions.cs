using UnityEngine;
using System.Collections;

public static class QuickUnityMathExtensions {

    public static Vector3 Round(this Vector3 vector) {
        return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
    }

    public static Vector3 Clamp(this Vector3 v, float length) {
        float m = v.magnitude;
        if (m > length) {
            return (v / m) * length;
        }
        return v;
    }

    public static Vector3 Snap(this Vector3 v, float cellSize) {
        v = (v / cellSize);
        return new Vector3((int)v.x, (int)v.y, (int)v.z) * cellSize;
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

    public static float Sqr(this float v) {
        return v * v;
    }
}
