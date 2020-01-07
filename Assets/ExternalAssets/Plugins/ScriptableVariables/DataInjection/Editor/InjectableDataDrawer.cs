using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DataInjectorDataEntry))]
public class InjectableDataDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, label);

		// Reset indent
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Get properties
		SerializedProperty key = property.FindPropertyRelative("key");
		SerializedProperty variableToReplace = property.FindPropertyRelative("variableToReplace");
		SerializedProperty replacementVariable = property.FindPropertyRelative("replacementVariable");

		Rect keyRect = new Rect(position.x, position.y, position.width * (2.5f / 6f), position.height);
		EditorGUI.PropertyField(keyRect, key, GUIContent.none);

		Rect variableToReplaceRect = new Rect(keyRect.xMax, keyRect.y, position.width * (2.5f / 6f), position.height);
		EditorGUI.PropertyField(variableToReplaceRect, variableToReplace, GUIContent.none);

		bool guiWasEnabled = GUI.enabled;
		GUI.enabled = false;
		Rect replacementVariableRect = new Rect(variableToReplaceRect.xMax, variableToReplaceRect.y, position.width * (1f / 6f), position.height);
		EditorGUI.ObjectField(replacementVariableRect, replacementVariable, GUIContent.none);
		GUI.enabled = guiWasEnabled;

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
