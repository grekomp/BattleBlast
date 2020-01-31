using UnityEngine;
using UnityEditor;

public class ScriptableObjectButtonDrawer<T> : PropertyDrawer where T : ScriptableObject {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		int buttonWidth = 30;
		Rect propertyFieldRect = position;
		if (property.objectReferenceValue == null) {
			propertyFieldRect.xMax -= buttonWidth;
			propertyFieldRect.width -= buttonWidth;
		}

		EditorGUI.PropertyField(propertyFieldRect, property, label);

		if (property.objectReferenceValue == null && GUI.Button(new Rect(propertyFieldRect.xMax, position.y, buttonWidth, position.height), new GUIContent("+", "Create a temporary instance"))) {
			T createdObject = ScriptableObject.CreateInstance<T>();
			property.objectReferenceValue = createdObject;
			property.serializedObject.ApplyModifiedProperties();
		}

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(DataBundle))]
public class DataBundleDrawer : ScriptableObjectButtonDrawer<DataBundle> { }
