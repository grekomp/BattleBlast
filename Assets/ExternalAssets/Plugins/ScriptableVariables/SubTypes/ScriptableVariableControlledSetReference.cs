using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ScriptableVariableControlledSetReference : ScriptableVariableReference<ScriptableVariableControlledSet, ScriptableVariableControlledSetVariable> {
	public ScriptableVariableControlledSetReference() : base() { }
	public ScriptableVariableControlledSetReference(ScriptableVariableControlledSet value) : base(value) { }
	public ScriptableVariableControlledSetReference(ScriptableVariableControlledSetVariable variable) : base(variable) { }

	public override void OnAfterDeserialize() {
		base.OnAfterDeserialize();

		Value.OnChanged -= HandleChange;
		Value.OnChanged += HandleChange;
	}

	public override void GatherReplaceableVariables(ref List<ScriptableVariable> replaceableVariables) {
		base.GatherReplaceableVariables(ref replaceableVariables);

		foreach (var variable in Value.Elements) {
			if (variable != null) {
				replaceableVariables.Add(variable);
			}
		}
	}
	public override bool TryReplaceVariable(ScriptableVariable from, ScriptableVariable to) {
		if (base.TryReplaceVariable(from, to)) return true;

		// Try to replace contained variables
		bool replacedAny = false;
		for (int i = 0; i < Value.Elements.Count; i++) {
			if (Value.ElementAt(i).IsAReplacement(from, to)) {
				Value.Replace(i, to);
				replacedAny = true;
			}
		}

		//if (replacedAny) HandleChange();

		return replacedAny;
	}
}
