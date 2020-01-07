using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class EditorUtilityAddons {

	public static void CreateScriptAsset(string templatePath, string destName) {
		typeof(ProjectWindowUtil)
			.GetMethod("CreateScriptAsset", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
			.Invoke(null, new object[] { templatePath, destName });
	}
}
