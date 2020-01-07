using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Variables/ToggleGroup")]
public class ToggleGroupVariable : ScriptableVariable<ToggleGroup> {
	public static ToggleGroupVariable New(ToggleGroup value = default) {
		var createdVariable = CreateInstance<ToggleGroupVariable>();
		createdVariable.Value = value;
		return createdVariable;
	}
}
