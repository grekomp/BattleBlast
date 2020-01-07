using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ControlledSet), true)]
public class ControlledSetEditor : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, property.FindPropertyRelative("elements"), label, true);
		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("elements"), label, true);
	}
}
