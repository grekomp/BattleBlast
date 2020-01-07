using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class DataInjectorDataEntry {
	public StringReference key = new StringReference();
	public ScriptableVariable variableToReplace;
	public ScriptableVariable replacementVariable;
}
