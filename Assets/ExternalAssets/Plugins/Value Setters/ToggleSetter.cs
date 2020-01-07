using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSetter : ValueSetter {
	[Header("Components")]
	public Toggle toggle;

	[Header("Variables")]
	public BoolReference value;
	public ToggleGroupReference toggleGroup;

	[Header("Events")]
	public GameEventHandler onValueChanged;
	public GameEventHandler onValueTrue;
	public GameEventHandler onValueFalse;

	protected override void ApplySet() {
		toggle.isOn = value;
		toggle.group = toggleGroup;
	}

	protected override void Init() {
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(HandleToggleValueChanged);
	}

	protected void HandleToggleValueChanged(bool newValue) {
		if (value.Value != newValue) {
			value.Value = newValue;
			onValueChanged?.Raise(this, newValue);
			if (value)
				onValueTrue?.Raise(this);
			else
				onValueFalse?.Raise(this);
		}
	}

	protected override void InitValueChangedHandlers() {
		value.OnChanged += HandleValueChanged;
		toggleGroup.OnChanged += HandleValueChanged;
	}
}
