using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class InspectorNavigator : EditorWindow {

    private static InspectorNavigator openedInstance;

    private List<int[]> backList = new List<int[]>();
    private List<int[]> forwardList = new List<int[]>();

    private int[] previousSelection;
    private bool isGoingBack;
    private bool isGoingForward;

    [MenuItem("Window/Inspector Navigator/Open")]
    private static void Create() {
        if (openedInstance) {
            Debug.LogWarning("The inspector navigator is already open! Restarting the window...");
            openedInstance.Close();
            openedInstance = null;
        }

        InspectorNavigator window = (InspectorNavigator)GetWindow(typeof(InspectorNavigator));
        window.Show();
        window.titleContent = new GUIContent("Navigator");
        window.minSize = new Vector2(10f, EditorGUIUtility.singleLineHeight + 5f);

        openedInstance = window;
    }

    [MenuItem("Window/Inspector Navigator/Back %LEFT")]
    private static void BackShortcut() {
        if (!openedInstance) {
            Debug.LogWarning("Open the navigator window first...");
            return;
        }
        openedInstance.Back();
    }

    [MenuItem("Window/Inspector Navigator/Forward %RIGHT")]
    private static void ForwardShortcut() {
        if (!openedInstance) {
            Debug.LogWarning("Open the navigator window first...");
            return;
        }
        openedInstance.Forward();
    }

    private void OnSelectionChange() {
        if (Selection.instanceIDs != previousSelection) {
            if (isGoingBack && backList.Count > 0) {
                if (previousSelection != null) {
                    forwardList.Add(previousSelection);
                }
                backList.RemoveAt(backList.Count - 1);
            } else if (isGoingForward && forwardList.Count > 0) {
                if (previousSelection != null) {
                    backList.Add(previousSelection);
                }
                forwardList.RemoveAt(forwardList.Count - 1);
            } else if (previousSelection != null) {
                backList.Add(previousSelection);
                forwardList.Clear();
            }
        }

        if (backList.Count > 64) {
            backList.RemoveAt(0);
        }

        isGoingBack = false;
        isGoingForward = false;
        previousSelection = Selection.instanceIDs;
    }

    private void OnDestroy() {
        openedInstance = null;
    }

    protected virtual void OnGUI() {
        GUILayout.BeginHorizontal();
        DrawGUIToolbar();
        GUILayout.EndHorizontal();
    }

    protected virtual void DrawGUIToolbar() {
        if (GUILayout.Button("◄")) {
            Back();
        }
        if (GUILayout.Button("►")) {
            Forward();
        }
    }

    private void Forward() {
        if (forwardList.Count == 0) {
            Debug.LogWarning("Nothing to go forward to...");
            return;
        }

        isGoingForward = true;
        Selection.instanceIDs = forwardList.Last();
    }

    private void Back() {
        if (backList.Count == 0) {
            Debug.LogWarning("Nothing to go back to...");
            return;
        }

        isGoingBack = true;
        Selection.instanceIDs = backList.Last();
    }
}