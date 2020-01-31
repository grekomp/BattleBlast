using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomPropertyDrawer(typeof(ScriptableVariable), true)]
public class ScriptableVariableDrawer : PropertyDrawer
{

	// Config
	public string newFileName = "New ScriptableVariable";
	int objectFieldWidth = 80;
	//float objectFieldFractionalWidth = 0.5f;
	int buttonWidth = 30;
	int minWidthToDrawAdditionalControls = 160;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		if (label.text != "")
		{
			position = EditorGUI.PrefixLabel(position, label);
		}

		DrawProperty(position, property);

		EditorGUI.EndProperty();
	}

	private void DrawProperty(Rect position, SerializedProperty property)
	{
		bool drawAdditionalControls = position.width > minWidthToDrawAdditionalControls;

		Rect propertyFieldRect = position;
		if (drawAdditionalControls)
		{
			propertyFieldRect.width = objectFieldWidth;
		}
		EditorGUI.PropertyField(propertyFieldRect, property, GUIContent.none);

		if (drawAdditionalControls)
		{
			Rect additionalControlsRect = position;
			additionalControlsRect.xMin = propertyFieldRect.xMax;
			DrawAdditionalControls(additionalControlsRect, property);
		}
	}

	private void DrawAdditionalControls(Rect position, SerializedProperty property)
	{
		if (property.objectReferenceValue == null)
		{
			DrawCreationControls(position, property);
		}
		else
		{
			DrawVariableValue(position, property);
		}
	}

	private void DrawVariableValue(Rect position, SerializedProperty property)
	{
		SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);

		var valueProperty = serializedObject.FindProperty("value");

		GUIContent label = GUIContent.none;
		if (valueProperty.hasVisibleChildren)
		{
			position.xMin += 10;
			label = new GUIContent("Value");
		}

		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, serializedObject.FindProperty("value"), label, true);
		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
	}

	private void DrawCreationControls(Rect position, SerializedProperty property)
	{
		Rect textFieldRect = position;
		textFieldRect.xMax = position.xMax - buttonWidth;

		newFileName = EditorGUI.TextField(textFieldRect, newFileName);

		if (property.objectReferenceValue == null && GUI.Button(new Rect(textFieldRect.xMax, position.y, buttonWidth, position.height), "+"))
		{
			property.objectReferenceValue = CreateNewObject(property);

			property.serializedObject.ApplyModifiedProperties();
		}
	}

	private UnityEngine.Object CreateNewObject(SerializedProperty property)
	{
		ScriptableObject newObject = ScriptableObject.CreateInstance(fieldInfo.FieldType);

		AssetDatabase.CreateAsset(newObject, Path.Combine(ScriptableVariablesConfig.Instance.variablesSavePath, newFileName + ".asset"));
		AssetDatabase.SaveAssets();

		return newObject;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (property.objectReferenceValue == null)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		else
		{
			SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);

			var valueProperty = serializedObject.FindProperty("value");
			return EditorGUI.GetPropertyHeight(valueProperty, true);
		}
	}
}