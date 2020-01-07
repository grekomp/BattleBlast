using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ToggleGroupReference : ScriptableVariableReference<ToggleGroup, ToggleGroupVariable> {
	public ToggleGroupReference() : base() { }
	public ToggleGroupReference(ToggleGroup value) : base(value) { }
	public ToggleGroupReference(ToggleGroupVariable variable) : base(variable) { }
}
