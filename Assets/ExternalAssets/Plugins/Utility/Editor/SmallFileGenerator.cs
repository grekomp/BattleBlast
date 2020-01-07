using UnityEditor;
using System.IO;
using System.Text;

public static class SmallFileGenerator  {

	public static void GenerateEnumOneFolderUp(string generatorClassName, string enumName, string[] values) {
		string directory = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(generatorClassName)[0]);
		directory = directory.Substring(0, directory.LastIndexOf('/'));
		directory = directory.Substring(0, directory.LastIndexOf('/'));
		string path = string.Format("{0}/{1}.cs", directory, enumName);

		using (StreamWriter streamWriter = new StreamWriter(path, false)) {
			streamWriter.WriteLine(string.Format("public enum {0} {{", enumName));
			for (int i = 0; i < values.Length; i++) {
				streamWriter.WriteLine(string.Format("\t{0} = {1},", values[i], i));
			}
			streamWriter.WriteLine("}");
		}
	}

	public static void GenerateStringArrayOneFolderUp(string generatorClassName,
		string className, string arrayName, string[] values) {

		string directory = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(generatorClassName)[0]);
		directory = directory.Substring(0, directory.LastIndexOf('/'));
		directory = directory.Substring(0, directory.LastIndexOf('/'));
		string path = string.Format("{0}/{1}.cs", directory, className);

		using (StreamWriter streamWriter = new StreamWriter(path, false)) {
			streamWriter.WriteLine("using System.Collections.Generic;");
			streamWriter.WriteLine("public static class {0} {{ ", className);
			streamWriter.WriteLine("\tpublic static string[] {0} = {{", arrayName);
			for (int i = 0; i < values.Length; i++) {
				streamWriter.WriteLine(string.Format("\t\t\"{0}\",", values[i]));
			}
			streamWriter.WriteLine("\t};");
			streamWriter.WriteLine("}");
		}
	}

    public static string RemoveSpecialCharacters(string str) {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str) {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_') {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
