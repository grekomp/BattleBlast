using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

[Serializable]
public class ScriptableVariable : ScriptableObject {
	public virtual void Awake() { }

	public virtual bool IsReplaceableBy(ScriptableVariable to) => false;
	public virtual bool IsAReplacement(ScriptableVariable from, ScriptableVariable to) => false;
	public virtual bool CopyValueFrom(ScriptableVariable scriptableVariable) => false;

	public virtual ScriptableVariable CreateCopy() => null;

	public virtual object ValueObject => null;
	public virtual string ValueString() => null;

	#region Change Events
	public event Action OnChanged;

	public virtual void HandleChange() {
		InvokeOnChanged();
	}
	protected void InvokeOnChanged() {
		OnChanged?.Invoke();
	}
	#endregion

	#region Saving and loading
	public virtual SaveToken GetSaveToken() {
		throw new NotImplementedException("ScriptableVariable: GetSaveToken was called on an object of type ScriptableVariable instead of a child class. This is not supported.");
	}
	public virtual void DeserializeFromSaveToken(string serializedSaveToken) {
		throw new NotImplementedException("ScriptableVariable: DeserializeFromSaveToken was called on an object of type ScriptableVariable instead of a child class. This is not supported.");
	}
	#endregion
}

[Serializable]
public abstract class ScriptableVariable<T> : ScriptableVariable {
	[SerializeField]
	protected T baseValue;
	[SerializeField]
	protected T value;
	public T Value {
		get {
			if (logValueGet) Debug.Log(string.Format("ScriptableVariable: Value.get ({0})", name), this);
			return value;
		}
		set {
			if (logValueSet) Debug.Log(string.Format("ScriptableVariable: Value.set ({0}) ({1})", name, value), this);
			this.value = value;
		}
	}


	#region Runtime creation
	public static ScriptableVariable New() {
		return CreateInstance<ScriptableVariable<T>>();
	}
	public override ScriptableVariable CreateCopy() {
		ScriptableVariable copiedVariable = CreateInstance(this.GetType()) as ScriptableVariable;
		copiedVariable.CopyValueFrom(this);
		return copiedVariable;
	}
	#endregion

	#region Change Events
	public event Action<ScriptableVariable<T>, T> OnVariableValueChanged;
	public event Action<T> OnValueChanged;

	[ContextMenu("HandleChange")]
	public override void HandleChange() {
		if (logChanges) Debug.Log(string.Format("ScriptableVariable ({0}) changed: {1}", GetType().Name, this.name), this);

		OnVariableValueChanged?.Invoke(this, value);
		OnValueChanged?.Invoke(value);
		base.HandleChange();
	}
	public void OnValidate() {
		HandleChange();
	}
	#endregion
	#region Reseting value
	public bool resetValueOnStartup = false;
	public override void Awake() {
		if (resetValueOnStartup) {
			ResetValue();
		}
	}

	[ContextMenu("ResetValue")]
	protected virtual void ResetValue() {
		value = baseValue;

#if UNITY_EDITOR
		if (Application.isPlaying)
			UnityEditor.Undo.RecordObject(this, "Reset value");
#endif
	}
	#endregion

	#region Debugging 
	[Header("Debugging")]
	public bool logValueGet = false;
	public bool logValueSet = false;
	[Space]
	public bool logChanges = false;
	#endregion

	#region Helper methods
	public override object ValueObject => Value as object;
	public override string ValueString() {
		return Value.ToString();
	}
	public override bool IsReplaceableBy(ScriptableVariable to) {
		return to as ScriptableVariable<T>;
	}
	public override bool IsAReplacement(ScriptableVariable from, ScriptableVariable to) {
		return IsReplaceableBy(to) && from == this;
	}
	public override bool CopyValueFrom(ScriptableVariable scriptableVariable) {
		if (scriptableVariable is ScriptableVariable<T> castedVariable) {
			Value = castedVariable.Value;
			return true;
		}

		return false;
	}
	#endregion
	#region Saving and loading
	public override SaveToken GetSaveToken() {
		return new SaveToken<T>().CreateFrom(this);
	}
	public override void DeserializeFromSaveToken(string serializedSaveToken) {
		SaveToken<T> saveToken = JsonUtility.FromJson<SaveToken<T>>(serializedSaveToken);

		saveToken.UpdateVariable(this);
	}

	#endregion
}