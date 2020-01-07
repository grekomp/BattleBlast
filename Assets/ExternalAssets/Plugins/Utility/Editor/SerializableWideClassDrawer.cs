using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(SerializableWideClass), true)]
public class SerializableWideClassDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		if (property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true)) {
			Rect childrenRect = new Rect(30, position.y + EditorGUIUtility.singleLineHeight, position.width + position.x - 30, EditorGUIUtility.singleLineHeight);
			IterateChildProperties(childrenRect, property);
		}

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.EndProperty();
	}

	private void IterateChildProperties(Rect childrenRect, SerializedProperty property) {
		SerializedProperty endProperty = property.GetEndProperty();

		var childProperty = property;
		if (childProperty.hasVisibleChildren) {
			childProperty.NextVisible(true);

			Rect childRect = childrenRect;
			do {
				childRect.height = EditorGUI.GetPropertyHeight(childProperty, true);

				EditorGUI.PropertyField(childRect, childProperty, true);

				childRect = new Rect(childRect.x, childRect.yMax, childRect.width, EditorGUIUtility.singleLineHeight);
			} while (childProperty.NextVisible(false) && SerializedProperty.EqualContents(childProperty, endProperty) == false);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (property.isExpanded) {
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		return EditorGUIUtility.singleLineHeight;
	}
}
