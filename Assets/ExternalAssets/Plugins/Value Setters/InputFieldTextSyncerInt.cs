using UnityEngine;
using System.Collections;
using TMPro;

public class InputFieldTextSyncerInt : ValueSyncer {
    [Header("Components")]
    public TMP_InputField inputField;

    [Header("Variables")]
    [HandleChanges] public IntReference intText;

    [Header("Events")]
    public GameEventHandler OnValueChanged;
    public GameEventHandler OnEndEdit;

    protected override void ApplySet() {
        if(inputField.text != intText.ToString())
            inputField.text = intText.Value.ToString();
    }
    protected override void ApplyGet() {
        int result = 0;
        int.TryParse(inputField.text,out result);
        intText.Value = result;
    }

    protected override void Init() {
        if(inputField == null) inputField = GetComponent<TMP_InputField>();
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
        OnValueChanged?.Raise(this, intText.Value);
    }
    protected void HandleEndEdit(string newValue) {
        HandleInputChanged();
        OnEndEdit?.Raise(this, newValue);
    }
    #endregion

    public void RaiseEvent() {
        OnValueChanged.Raise(this);
    }
}

