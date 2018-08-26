using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Asserts {
    public static void NotNull(object obj, string failureMessage = "") {
        if (obj == null) {
            throw new NullReferenceException("Not Null Assertion failed: " + failureMessage);
        }
    }

    public static void Null(object obj, string failureMessage = "") {
        if (obj != null) {
            throw new Exception("Null Assertion failed: " + failureMessage);
        }
    }

    public static void AssertTrue(bool expression, string failureMessage) {
        if (!expression) {
            throw new Exception("Assertion failed: " + failureMessage);
        }
    }

    public static void WeakAssertTrue(bool expression, string failureMessage) {
        if (!expression) {
            Debug.LogError("Weak Assertion failed: " + failureMessage);
        }
    }
}
