using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(EventKeyPair))]
public class EventKeyPairDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, label);

		// Reset indent
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Get properties
		SerializedProperty key = property.FindPropertyRelative("key");
		SerializedProperty gameEvent = property.FindPropertyRelative("gameEvent");

		Rect keyRect = new Rect(position.x, position.y, position.width / 2, position.height);
		EditorGUI.PropertyField(keyRect, key, GUIContent.none);

		Rect dataRect = new Rect(keyRect.xMax, keyRect.y, position.width / 2, position.height);
		EditorGUI.PropertyField(dataRect, gameEvent, GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
