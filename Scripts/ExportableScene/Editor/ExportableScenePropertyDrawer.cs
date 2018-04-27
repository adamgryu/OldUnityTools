using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(ExportableScene))]
public class ExportableSceneDrawer : PropertyDrawer {

    private bool verified = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        var sceneReference = property.FindPropertyRelative("sceneReference");
        Object lastSceneObj = sceneReference.objectReferenceValue;
        Object sceneObj = EditorGUI.ObjectField(contentPosition, sceneReference.objectReferenceValue, typeof(SceneAsset), false);
        sceneReference.objectReferenceValue = sceneObj;

        EditorGUI.EndProperty();

        if (lastSceneObj != sceneObj || !verified) {
            verified = true;

            // Update the scene name.
            string name = sceneObj == null ? "" : sceneObj.name;
            var sceneNameProperty = property.FindPropertyRelative("sceneName");
            if (sceneNameProperty.stringValue != name) {
                Debug.Log("Setting the exportable scene from " + sceneNameProperty.stringValue + " to " + name);
                sceneNameProperty.stringValue = name;
            }
        }
    }
}
