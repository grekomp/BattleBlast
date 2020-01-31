using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DataBundleCollection))]
public class DataBundleCollectionDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();

		//EditorGUI.PropertyField(position, property.FindPropertyRelative("dataBundles"), true);

		Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		if (property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true)) {
			Rect childrenRect = new Rect(30, position.y + EditorGUIUtility.singleLineHeight, position.width + position.x - 30, EditorGUI.GetPropertyHeight(property.FindPropertyRelative("dataBundles"), label, true));
			EditorGUI.PropertyField(childrenRect, property.FindPropertyRelative("dataBundles"), true);
		}

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (property.isExpanded) {
			return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("dataBundles"), label, true) + EditorGUIUtility.singleLineHeight;
		}

		return EditorGUIUtility.singleLineHeight;
	}
}