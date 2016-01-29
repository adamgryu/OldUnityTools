using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(ExportableScene))]
public class ExportableSceneDrawer : PropertyDrawer {

    private Object sceneObject;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        if (this.sceneObject == null) {
            this.sceneObject = property.FindPropertyRelative("sceneReference").objectReferenceValue;
        }

        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("sceneReference"), GUIContent.none);        
        EditorGUI.EndProperty();

        Object obj = property.FindPropertyRelative("sceneReference").objectReferenceValue;
        if (obj != this.sceneObject) {
            this.sceneObject = obj;
            if (obj != null) {
                Debug.Log("Setting " + obj.name + " to the Exportable Scene!");
                property.FindPropertyRelative("sceneName").stringValue = obj.name;
            } else {
                Debug.Log("Setting the Exportable Scene to NULL!");
            }
        }
    }
}
