using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataKeyPair))]
public class DataKeyPairDrawer : PropertyDrawer {
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
		SerializedProperty key = property.FindPropertyRelative("key");
		SerializedProperty data = property.FindPropertyRelative("data");

		Rect keyRect = new Rect(position.x, position.y, position.width / 2, position.height);
		EditorGUI.PropertyField(keyRect, key, GUIContent.none);

		Rect dataRect = new Rect(keyRect.xMax, keyRect.y, position.width / 2, position.height);
		EditorGUI.PropertyField(dataRect, data, GUIContent.none);

		EditorGUI.indentLevel = indent;

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.EndProperty();
	}
}
