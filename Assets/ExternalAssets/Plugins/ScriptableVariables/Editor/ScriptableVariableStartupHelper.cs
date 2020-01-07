using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public class ScriptableVariableStartupHelper {
	static ScriptableVariableStartupHelper() {
		List<ScriptableVariable> variables = Resources.FindObjectsOfTypeAll<ScriptableVariable>().ToList();

		foreach (var variable in variables) {
			variable.Awake();
		}
	}
}