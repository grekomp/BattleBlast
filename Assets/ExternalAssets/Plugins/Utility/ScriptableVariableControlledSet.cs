using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ScriptableVariableControlledSet : ControlledSet<ScriptableVariable> {
	public event Action OnChanged;

	protected override void HandleCollectionUpdated() {
		base.HandleCollectionUpdated();
		HandleChanged();
	}
	protected override void HandleElementAdded(ScriptableVariable element) {
		base.HandleElementAdded(element);

		if (element)
			element.OnChanged += HandleChanged;
	}
	protected override void HandleElementRemoved(ScriptableVariable element) {
		if (element)
			element.OnChanged -= HandleChanged;

		base.HandleElementRemoved(element);
	}

	public void HandleChanged() {
		OnChanged?.Invoke();
	}
}
