using UnityEngine;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ScriptableVariablesConfig : ScriptableObject
{
	#region Singleton
	protected static ScriptableVariablesConfig _instance;
	public static ScriptableVariablesConfig Instance {
		get {
			Initialize();

			return _instance;
		}
	}

#if UNITY_EDITOR
	[InitializeOnLoadMethod]
#else
	[RuntimeInitializeOnLoadMethod]
#endif
	protected static void Initialize()
	{
		if (_instance == null)
		{
			var found = Resources.LoadAll<ScriptableVariablesConfig>("");
			_instance = found.FirstOrDefault();

			if (_instance == null)
			{
				Debug.LogWarning("ScriptableVariablesConfig not found, creating a new config file.");
				_instance = CreateNewInstance();
			}
		}
	}

	private static ScriptableVariablesConfig CreateNewInstance()
	{
		ScriptableObject newObject = CreateInstance<ScriptableVariablesConfig>();
#if UNITY_EDITOR
		if (AssetDatabase.IsValidFolder("Assets/Resources") == false)
		{
			AssetDatabase.CreateFolder("Assets", "Resources");
		}

		AssetDatabase.CreateAsset(newObject, "Assets/Resources/ScriptableVariablesConfig.asset");
		AssetDatabase.SaveAssets();
#endif
		return newObject as ScriptableVariablesConfig;
	}
	#endregion

	public StringReference variablesSavePath = new StringReference("Assets/Resources/Variables/");
}
