using UnityEngine;
using System;
using System.Text;

public struct Vector2Int {

    public int x;
    public int y;

    public Vector2Int(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public bool IsNeighbors(Vector2Int other) {
        return Math.Abs(other.x - this.x) <= 1 && Math.Abs(other.y - this.y) <= 1;
    }

    public Vector3 ToV3() {
        return new Vector3(x, y, 0);
    }

    public override bool Equals(object obj) {
        return (obj is Vector2Int) ? this == ((Vector2Int)obj) : false;
    }

    public bool Equals(Vector2Int other) {
        return this == other;
    }

    public override int GetHashCode() {
        return this.x + this.y;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder(24);
        sb.Append("{X:");
        sb.Append(this.x);
        sb.Append(" Y:");
        sb.Append(this.y);
        sb.Append("}");
        return sb.ToString();
    }

    #region operators

    public static Vector2Int operator -(Vector2Int value) {
        value.x = -value.x;
        value.y = -value.y;
        return value;
    }


    public static bool operator ==(Vector2Int value1, Vector2Int value2) {
        return value1.x == value2.x && value1.y == value2.y;
    }


    public static bool operator !=(Vector2Int value1, Vector2Int value2) {
        return value1.x != value2.x || value1.y != value2.y;
    }


    public static Vector2Int operator +(Vector2Int value1, Vector2Int value2) {
        value1.x += value2.x;
        value1.y += value2.y;
        return value1;
    }


    public static Vector2Int operator -(Vector2Int value1, Vector2Int value2) {
        value1.x -= value2.x;
        value1.y -= value2.y;
        return value1;
    }


    public static Vector2Int operator *(Vector2Int value1, Vector2Int value2) {
        value1.x *= value2.x;
        value1.y *= value2.y;
        return value1;
    }


    public static Vector2Int operator *(Vector2Int value, int scaleFactor) {
        value.x *= scaleFactor;
        value.y *= scaleFactor;
        return value;
    }


    public static Vector2Int operator *(int scaleFactor, Vector2Int value) {
        value.x *= scaleFactor;
        value.y *= scaleFactor;
        return value;
    }


    public static Vector2Int operator /(Vector2Int value1, Vector2Int value2) {
        value1.x /= value2.x;
        value1.y /= value2.y;
        return value1;
    }


    public static Vector2Int operator /(Vector2Int value1, int divider) {
        float factor = 1 / divider;
        value1.x = (int)(value1.x * factor);
        value1.y = (int)(value1.y * factor);
        return value1;
    }

    #endregion Operators
}
