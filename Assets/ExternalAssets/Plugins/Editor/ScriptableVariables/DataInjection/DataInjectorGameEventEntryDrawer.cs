using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataInjectorGameEventEntry))]
public class DataInjectorGameEventEntryDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, label);

		// Reset indent
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Get properties
		SerializedProperty key = property.FindPropertyRelative("key");
		SerializedProperty eventToReplace = property.FindPropertyRelative("eventToReplace");
		SerializedProperty replacementEvent = property.FindPropertyRelative("replacementEvent");

		Rect keyRect = new Rect(position.x, position.y, position.width / 3, position.height);
		EditorGUI.PropertyField(keyRect, key, GUIContent.none);

		Rect eventToReplaceRect = new Rect(keyRect.xMax, keyRect.y, position.width / 3, position.height);
		EditorGUI.PropertyField(eventToReplaceRect, eventToReplace, GUIContent.none);

		bool guiWasEnabled = GUI.enabled;
		GUI.enabled = false;
		Rect replacementEventRect = new Rect(eventToReplaceRect.xMax, eventToReplaceRect.y, position.width / 3, position.height);
		EditorGUI.PropertyField(replacementEventRect, replacementEvent, GUIContent.none);
		GUI.enabled = guiWasEnabled;

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
