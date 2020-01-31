using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class ScriptableVariableGenerator : EditorWindow
{

	static string variableName = "";
	static string variablePath = "Assets";

	[MenuItem("Assets/Create/Scriptable Variables/New ScriptableVariable type")]
	static void Init()
	{
		string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (selectedPath != "") variablePath = selectedPath;

		ScriptableVariableGenerator window = EditorWindow.GetWindow<ScriptableVariableGenerator>();
		window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 90);
		window.Show();
	}

	void OnGUI()
	{
		variableName = EditorGUILayout.TextField("Variable Type", variableName);
		variablePath = EditorGUILayout.TextField("Variable Path", variablePath);

		if (GUILayout.Button("Create"))
		{
			Create();
			Close();
		}

		GUILayout.Space(10);
		if (GUILayout.Button("Close"))
		{
			Close();
		}
	}

	static void Create()
	{
		if (File.Exists(variablePath)) variablePath = Path.GetDirectoryName(variablePath);

		AssetDatabase.StartAssetEditing();
		AssetDatabase.ImportAsset(CreateScriptableVariableScript(variableName, variablePath));
		AssetDatabase.ImportAsset(CreateScriptableVariableReferenceScript(variableName, variablePath));
		//AssetDatabase.ImportAsset(CreateScriptableVariableReferenceDrawerScript(variableName, variablePath));
		AssetDatabase.StopAssetEditing();
		AssetDatabase.Refresh();
	}

	static string CreateScriptableVariableScript(string variableType, string path)
	{
		string upperCaseType = variableType.FirstCharToUpper();

		path = Path.Combine(path, upperCaseType + "Variable.cs");
		using (StreamWriter file = new StreamWriter(path))
		{
			file.WriteLine("using UnityEngine;");
			file.WriteLine(string.Format("[CreateAssetMenu(menuName = \"Scriptable Variables/{0}\")]", upperCaseType));
			file.WriteLine(string.Format("public class {0}Variable : ScriptableVariable<{1}> {{", upperCaseType, variableType));
			file.WriteLine(string.Format("    public static {0}Variable New({1} value = default) {{", upperCaseType, variableType));
			file.WriteLine(string.Format("        var createdVariable = CreateInstance<{0}Variable>();", upperCaseType));
			file.WriteLine("        createdVariable.Value = value;");
			file.WriteLine("        return createdVariable;");
			file.WriteLine("    }");
			file.WriteLine("}");
		}

		return path;
	}
	static string CreateScriptableVariableReferenceScript(string variableType, string path)
	{
		string upperCaseType = variableType.FirstCharToUpper();

		path = Path.Combine(path, upperCaseType + "Reference.cs");
		if (File.Exists(path) == false)
		{
			using (StreamWriter file = new StreamWriter(path))
			{
				file.WriteLine("using System;");
				file.WriteLine("using UnityEngine;");

				file.WriteLine("[Serializable]");
				file.WriteLine(string.Format("public class {0}Reference : ScriptableVariableReference<{1}, {0}Variable> {{", upperCaseType, variableType));
				file.WriteLine(string.Format("    public {0}Reference() : base() {{ }}", upperCaseType));
				file.WriteLine(string.Format("    public {0}Reference({1} value) : base(value) {{ }}", upperCaseType, variableType));
				file.WriteLine(string.Format("    public {0}Reference({0}Variable variable) : base(variable) {{ }}", upperCaseType));
				//file.WriteLine("");
				//file.WriteLine(string.Format("    public static implicit operator {0}Reference({1} val) {{", upperCaseType, variableType));
				//file.WriteLine(string.Format("        return new {0}Reference(val);", upperCaseType));
				//file.WriteLine(string.Format("    }}"));
				//file.WriteLine(string.Format("    public static implicit operator {0}Reference({0}Variable var) {{", upperCaseType));
				//file.WriteLine(string.Format("        return new {0}Reference(var);", upperCaseType));
				//file.WriteLine("    }");
				file.WriteLine("}");
			}
		}

		return path;
	}

	//[Obsolete("No longer needed")]
	//static string CreateScriptableVariableReferenceDrawerScript(string variableType, string path) {
	//	string upperCaseType = variableType.FirstCharToUpper();

	//	path = Path.Combine(path, "Editor");
	//	path = Path.Combine(path, upperCaseType + "ReferenceDrawer.cs");

	//	if (File.Exists(path) == false) {
	//		using (StreamWriter file = new StreamWriter(path)) {
	//			file.WriteLine("using UnityEditor;");
	//			file.WriteLine("using UnityEngine;");

	//			file.WriteLine(string.Format("[CustomPropertyDrawer(typeof({0}Reference))]", upperCaseType));
	//			file.WriteLine(string.Format("public class {0}ReferenceDrawer : ScriptableReferenceDrawer {{ }}", upperCaseType));
	//		}
	//	}

	//	return path;
	//}
}