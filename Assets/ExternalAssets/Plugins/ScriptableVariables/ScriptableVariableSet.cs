using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Variables/ScriptableVariableSet")]
public class ScriptableVariableSet : ScriptableObject {
	/// <summary>
	/// A list of variables to be initialized on awake
	/// </summary>
	public List<ScriptableVariable> scriptableVariables = new List<ScriptableVariable>();

	public void Awake() {
		foreach (var scriptableVariable in scriptableVariables) {
			if (scriptableVariable != null) scriptableVariable.Awake();
		}
	}
}
