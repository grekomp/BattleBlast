using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;

[InitializeOnLoad]
public static class ProjectWindowListener {

	public static string lastSelectedFolderPath;

	static ProjectWindowListener() {
		lastSelectedFolderPath = "Assets";
		Selection.selectionChanged += OnSelectionChanged;
	}

	static void OnSelectionChanged() {
		Type pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.ProjectBrowser");
		if (pt != null && EditorWindow.focusedWindow != null) {
			if (EditorWindow.focusedWindow.GetType() == pt) {
				GetSelectedFolder();
			}
		}
	}

	static void GetSelectedFolder() {
		string path = "";
		UnityEngine.Object selected = Selection.activeObject;
		if (selected != null) {
			path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (!string.IsNullOrEmpty(path)) {
				path = Path.GetDirectoryName(path);
				lastSelectedFolderPath = path;
			}
		}
	}
}
