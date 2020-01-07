using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class EditorUtilityExtensions {

	public static void CreateScriptAsset(string templatePath, string destName) {
#if UNITY_EDITOR

		typeof(ProjectWindowUtil)
			.GetMethod("CreateScriptAsset", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
			.Invoke(null, new object[] { templatePath, destName });
#endif
	}

	/// <summary>
	/// A version of Undo.RecordObject that gets excluded from build
	/// </summary>
	public static void RecordObject(Object objectToUndo, string name = "No description") {
#if UNITY_EDITOR
		Undo.RecordObject(objectToUndo, name);
#endif
	}

	public static void RecordPrefabInstancePropertyModifications(Object targetObject) {

	}
}
