using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(GameEventHandlerGroup))]
public class GameEventHandlerGroupEditor : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, property.FindPropertyRelative("inspectedEvents"), new GUIContent(property.name), true);

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.EndProperty();
	}
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("inspectedEvents"), label, true);
	}
}
