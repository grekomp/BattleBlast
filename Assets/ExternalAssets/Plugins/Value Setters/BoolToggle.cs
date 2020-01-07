using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class BoolToggle : ValueSetter {
	[Header("Variables")]
	[HandleChanges] public BoolReference value;
	bool lastValue;
	public BoolReference onlySetIfValueDiffers = new BoolReference(true);

	[Header("Events")]
	public BoolUnityEvent onValueChanged;
	public UnityEvent onValueTrue;
	public UnityEvent onValueFalse;

	public void SetValue(bool newValue) {
		value.Value = newValue;
		CheckValue();
	}

	protected override void ApplySet() {
		CheckValue();
	}

	protected void CheckValue() {
		if (value != lastValue || onlySetIfValueDiffers.Value == false) {
			lastValue = value;
			onValueChanged?.Invoke(value);

			if (value)
				onValueTrue?.Invoke();
			else
				onValueFalse?.Invoke();
		}
	}

	protected override void Init() {
		lastValue = !value.Value;
	}
}
