using TMPro;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/FontStyles")]
public class FontStylesVariable : ScriptableVariable<FontStyles> {
	public static FontStylesVariable New(FontStyles value = default) {
		var createdVariable = CreateInstance<FontStylesVariable>();
		createdVariable.Value = value;
		return createdVariable;
	}
}
