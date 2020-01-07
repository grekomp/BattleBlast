using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScriptableVariableReference), true)]
public class ScriptableReferenceDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		// Draw label
		position = EditorGUI.PrefixLabel(position, label);

		// Reset indent
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Get properties
		SerializedProperty useConstant = property.FindPropertyRelative("useConstant");
		SerializedProperty constantValue = property.FindPropertyRelative("constantValue");
		SerializedProperty variable = property.FindPropertyRelative("variable");

		// Draw a toggle for switching between constant and variable
		Rect toggleRect = new Rect(position.x, position.y, 20, EditorGUIUtility.singleLineHeight);

		useConstant.boolValue = EditorGUI.Toggle(toggleRect, GUIContent.none, useConstant.boolValue);
		position.xMin = toggleRect.xMax;

		Rect valueRect = position;
		int idButtonWidth = 30;
		if (HasIdAttribute && fieldInfo.FieldType == typeof(StringReference)) {
			valueRect.xMax -= idButtonWidth;
		}

		if (useConstant.boolValue) {
			DrawConstantProperty(valueRect, constantValue);
		}
		else {
			DrawVariableProperty(valueRect, variable);
		}

		Rect idButtonRect = new Rect(valueRect.xMax, valueRect.y, idButtonWidth, EditorGUIUtility.singleLineHeight);
		if (HasIdAttribute && fieldInfo.FieldType == typeof(StringReference)) {
			DrawIdButton(idButtonRect, constantValue);
		}

		EditorGUI.indentLevel = indent;
		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}
		EditorGUI.EndProperty();
	}

	private void DrawIdButton(Rect idButtonRect, SerializedProperty constantValue) {
		if (GUI.Button(idButtonRect, new GUIContent("id", "Generate a new id"))) {
			string newId = Guid.NewGuid().ToString();
			constantValue.stringValue = newId;
		}
	}

	protected virtual void DrawConstantProperty(Rect valueRect, SerializedProperty property) {
		GUIContent label = GUIContent.none;
		if (property.hasVisibleChildren) {
			label = new GUIContent("Value");
			valueRect.xMin += 10;
		}
		valueRect.height = EditorGUI.GetPropertyHeight(property, true);
		EditorGUI.PropertyField(valueRect, property, label, true);
	}

	protected virtual void DrawVariableProperty(Rect valueRect, SerializedProperty variable) {
		valueRect.height = EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(valueRect, variable, GUIContent.none, true);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		// Get properties
		SerializedProperty useConstant = property.FindPropertyRelative("useConstant");

		SerializedProperty serializedProperty;
		if (useConstant.boolValue) {
			serializedProperty = property.FindPropertyRelative("constantValue");
		}
		else {
			serializedProperty = property.FindPropertyRelative("variable");
		}

		return EditorGUI.GetPropertyHeight(serializedProperty);
	}

	protected bool HasIdAttribute => fieldInfo.GetCustomAttributes(typeof(Id), true).Count() > 0;
}

public class ScriptableReferenceDrawer<T, K> : ScriptableReferenceDrawer where K : ScriptableVariableReference<T, ScriptableVariable<T>> {
}