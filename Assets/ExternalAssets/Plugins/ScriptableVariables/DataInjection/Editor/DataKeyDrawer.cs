using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataKey))]
public class DataKeyDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		// Save old indent
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = Mathf.Clamp(EditorGUI.indentLevel, 0, 2);
		EditorGUI.indentLevel = 0;

		// Draw label
		position = EditorGUI.PrefixLabel(position, label);

		// Get properties
		SerializedProperty variable = property.FindPropertyRelative("variable");
		SerializedProperty keyString = property.FindPropertyRelative("keyString");

		int variableRectSmallSize = 80;
		Rect variableRect = new Rect(position.x, position.y, position.width, position.height);
		if (variable.objectReferenceValue == null) {
			variableRect.width = variableRectSmallSize;
		}

		EditorGUI.PropertyField(variableRect, variable, GUIContent.none);

		if (variable.objectReferenceValue == null) {
			Rect keyStringRect = new Rect(variableRect.xMax, variableRect.y, position.width - variableRect.width, position.height);
			EditorGUI.PropertyField(keyStringRect, keyString, GUIContent.none);
		}
		EditorGUI.indentLevel = indent;

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.EndProperty();
	}
}

