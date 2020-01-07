using UnityEngine;
using System.Collections;
using TMPro;

public class InputFieldTextSyncer : ValueSyncer {
	[Header("Components")]
	public TMP_InputField inputField;

	[Header("Variables")]
	[HandleChanges] public StringReference text;

	[Header("Events")]
	public GameEventHandler OnValueChanged;
	public GameEventHandler OnEndEdit;

	protected override void ApplySet() {
		if (inputField.text != text)
			inputField.text = text;
	}
	protected override void ApplyGet() {
		text.Value = inputField.text;
	}

	protected override void Init() {
		if (inputField == null) inputField = GetComponent<TMP_InputField>();
	}

	#region Handling input events
	protected override void BindChangeEvents() {
		inputField.onValueChanged.AddListener(HandleInputChanged);
		inputField.onEndEdit.AddListener(HandleEndEdit);
	}
	protected override void UnbindChangeEvents() {
		inputField.onValueChanged.RemoveListener(HandleInputChanged);
		inputField.onEndEdit.RemoveListener(HandleEndEdit);
	}

	protected void HandleInputChanged(string newValue) {
		HandleInputChanged();
	}
	protected void HandleInputChanged() {
		Get();
		OnValueChanged?.Raise(this, text.Value);
	}
	protected void HandleEndEdit(string newValue) {
		HandleInputChanged();
		OnEndEdit?.Raise(this, newValue);
	}
	#endregion
}
