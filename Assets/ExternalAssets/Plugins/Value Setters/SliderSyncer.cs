using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSyncer : ValueSyncer {
	[Header("Components")]
	public Slider slider;

	[Header("Variables")]
	[HandleChanges] public DoubleReference normalizedValue;

	[Header("Events")]
	public GameEventHandlerGroup onValueChanged = new GameEventHandlerGroup();

	protected override void ApplySet() {
		slider?.SetValueWithoutNotify((float)normalizedValue);
	}
	protected override void ApplyGet() {
		normalizedValue.Value = slider.normalizedValue;
	}

	protected void HandleInputChanged(float normalized) {
		Get();
		onValueChanged?.Raise(this, normalized);
	}
	protected override void BindChangeEvents() {
		slider.onValueChanged.AddListener(HandleInputChanged);
	}
	protected override void UnbindChangeEvents() {
		slider.onValueChanged.RemoveListener(HandleInputChanged);
	}

	protected override void Init() {
		if (slider == null)
			slider = GetComponent<Slider>();
	}
}
