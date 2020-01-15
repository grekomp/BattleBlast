using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public abstract class ScriptableVariableReference
{
	#region Getting values
	public abstract ScriptableVariable GetValueAsVariable();
	public abstract ScriptableVariable GetValueAsNewVariable();
	#endregion

	#region Setting values
	protected abstract void SwitchToVariable(bool setValueIfSwitching = true, bool useTemporaryVariableIfNull = true);
	public abstract void SetAndUseVariable(ScriptableVariable variable);
	#endregion

	#region Change events
	public event Action OnChanged;

	public virtual void HandleChange()
	{
		HandleChanged();
	}
	protected void HandleChanged() => OnChanged?.Invoke();
	#endregion

	#region Replacing variables
	public abstract void GatherReplaceableVariables(ref List<ScriptableVariable> replaceableVariables);
	public abstract bool TryReplaceVariable(ScriptableVariable from, ScriptableVariable to);
	#endregion
}

[Serializable]
public class ScriptableVariableReference<T, K> : ScriptableVariableReference, ISerializationCallbackReceiver where K : ScriptableVariable<T>
{
	#region Value
	public bool useConstant = true;
	public T constantValue = default(T);

	protected K previousVariable;
	[SerializeField]
	protected K variable;
	public K Variable {
		get {
			return variable;
		}
		set {
			UsesTemporaryVariable = false;
			UnbindCurrentVariable();
			BindVariable(value);
		}
	}

	[SerializeField]
	[Disabled]
	protected bool _usesTemporaryVariable = false;
	public bool UsesTemporaryVariable { get { return _usesTemporaryVariable; } protected set { _usesTemporaryVariable = value; } }

	public T Value {
		get {
			if (useConstant == false)
			{
				if (Variable != null) return Variable.Value;

				if (UsesTemporaryVariable)
				{
					UsesTemporaryVariable = false;
					useConstant = true;
				}
			}

			return constantValue;
		}
		set {
			if (useConstant == false)
			{
				if (Variable != null)
				{
					Variable.Value = value;
				}
				else
				{
					useConstant = true;
					constantValue = value;
					UsesTemporaryVariable = false;
				}
			}
			else
			{
				constantValue = value;
			}

			HandleChange();
		}
	}
	#endregion

	#region Constructors
	public ScriptableVariableReference()
	{
		useConstant = true;
		constantValue = default(T);
	}
	public ScriptableVariableReference(T value)
	{
		SetAndUseValue(value);
	}
	public ScriptableVariableReference(K variable)
	{
		SetAndUseVariable(variable);
	}
	#endregion

	#region Setting and getting values
	public override void SetAndUseVariable(ScriptableVariable variable)
	{
		K castedVariable = variable as K;
		if (castedVariable)
		{
			Variable = castedVariable;
			useConstant = false;
			HandleChange();
		}
	}
	public void SetAndUseVariable(K scriptableVariable)
	{
		Variable = scriptableVariable;
		useConstant = false;
		HandleValueChange();
	}
	public void SetAndUseVariable(T value)
	{
		UseTemporaryVariable();
		variable.Value = value;
	}
	public void SetAndUseVariable()
	{
		UseTemporaryVariable();
	}
	public void SetAndUseValue(T value)
	{
		constantValue = value;
		useConstant = true;
		HandleChange();
	}

	/// <summary>
	/// Returns the currently used variable, or creates a new one.
	/// </summary>
	/// <returns>A ScriptableVariable containing the current value of this reference.</returns>
	public override ScriptableVariable GetValueAsVariable()
	{
		if (useConstant)
		{
			UseTemporaryVariable();
		}

		return Variable;
	}
	/// <summary>
	/// Creates a new variable with a copy of this references value.
	/// </summary>
	public override ScriptableVariable GetValueAsNewVariable()
	{
		K variable = ScriptableObject.CreateInstance<K>();
		variable.Value = Value;
		return variable;
	}

	protected void UseTemporaryVariable()
	{
		SetAndUseVariable(GetValueAsNewVariable());
		UsesTemporaryVariable = true;
	}

	/// <summary>
	/// Switches this reference to use a variable instead of contantValue. Creates a temporary variable if one is not already present.
	/// </summary>
	/// <param name="setValueFromConstantValueIfSwitching">If variable is not null, should this operation copy it's value from constantValue.</param>
	protected override void SwitchToVariable(bool setValueFromConstantValueIfSwitching = true, bool useTemporaryVariableIfNull = true)
	{
		if (useConstant)
		{
			if (Variable == null && useTemporaryVariableIfNull)
			{
				UseTemporaryVariable();
			}
			else
			{
				useConstant = false;
				if (setValueFromConstantValueIfSwitching) Value = constantValue;
			}

			HandleChange();
		}
	}
	/// <summary>
	/// Switches the reference to use a constant value instead of a variable
	/// </summary>
	public void SwitchToConstantValue(bool setValueFromVariableIfSwitching = true)
	{
		if (useConstant == false)
		{
			if (setValueFromVariableIfSwitching && Variable != null)
			{
				SetAndUseValue(Value);
			}
			else
			{
				useConstant = true;
			}

			HandleChange();
		}
	}
	#endregion

	#region Change events
	public event Action<T> OnValueChanged;
	public event Action<ScriptableVariable> OnVariableChanged;
	public event Action<ScriptableVariable<T>, T> OnVariableValueChanged;

	/// <summary>
	/// Raises change events on this scriptableVariableReference and the bound variable
	/// </summary>
	[ContextMenu("Handle Change")]
	public override void HandleChange()
	{
		if (useConstant)
		{
			HandleValueChange();
		}
		else
		{
			Variable?.HandleChange();
		}

		DebugChanged();
	}
	/// <summary>
	/// Raises change events on this scriptableVaribleReference only
	/// </summary>
	public void HandleValueChange()
	{
		HandleChanged();
		HandleValueChanged(Value);
	}

	protected void HandleValueChanged(T value) => OnValueChanged?.Invoke(value);
	protected void HandleVariableValueChanged(ScriptableVariable<T> variable, T value) => OnVariableValueChanged?.Invoke(variable, value);
	#endregion

	#region Binding variable change events
	protected void BindVariable(K value)
	{
		previousVariable = value;
		variable = value;

		if (Variable != null)
		{
			Variable.OnChanged += HandleChanged;
			Variable.OnValueChanged += HandleValueChanged;
			Variable.OnVariableValueChanged += HandleVariableValueChanged;
		}

		OnVariableChanged?.Invoke(Variable);
	}
	protected virtual void UnbindCurrentVariable()
	{
		if (previousVariable != null)
		{
			previousVariable.OnChanged -= HandleChanged;
			previousVariable.OnValueChanged -= HandleValueChanged;
			previousVariable.OnVariableValueChanged -= HandleVariableValueChanged;
		}
	}
	#endregion

	#region Replacing variables
	public override void GatherReplaceableVariables(ref List<ScriptableVariable> replaceableVariables)
	{
		if (Variable != null)
		{
			replaceableVariables.Add(Variable);
		}
	}
	public virtual void ReplaceVariable(ScriptableVariable from, K to)
	{
		if (Variable == from)
		{
			SetAndUseVariable(to);
			DebugVariableReplaced(from, to);
		}

		DebugVariableReplacementFailed(from, to);
	}
	public override bool TryReplaceVariable(ScriptableVariable from, ScriptableVariable to)
	{
		if (Variable != null && Variable.IsAReplacement(from, to))
		{
			SetAndUseVariable(to as K);
			DebugVariableReplaced(from, to);
			return true;
		}

		DebugVariableReplacementFailed(from, to);
		return false;
	}
	#endregion

	#region Serialization helpers
	public virtual void OnAfterDeserialize()
	{
		ValidateBoundVariable();
		HandleChange();
	}
	public virtual void OnBeforeSerialize()
	{
		ValidateBoundVariable();
	}
	public virtual void OnValidate()
	{
		ValidateBoundVariable();
		HandleChange();
	}

	private void ValidateBoundVariable()
	{
		if (UsesTemporaryVariable && variable == null)
		{
			UsesTemporaryVariable = false;
			useConstant = true;
		}

		if (previousVariable != variable)
		{
			Variable = variable;
		}
	}
	#endregion

	#region Debugging
	[SerializeField]
	protected bool debugEnabled = false;
	[SerializeField]
	protected bool debugLogAll = true;
	[SerializeField]
	protected bool debugVariableReplacements = true;
	[SerializeField]
	protected bool debugFailedVariableReplacements = false;
	[SerializeField]
	protected bool debugChanges = true;

	protected void DebugVariableReplaced(ScriptableVariable from, ScriptableVariable to)
	{
		if (debugEnabled && (debugLogAll || debugVariableReplacements))
		{
			Debug.Log(string.Format("{0} (ScriptableVariableReference): Replaced variable {1} with {2}. New value is {3}.", this, from, to, to.ValueObject));
		}
	}
	protected void DebugVariableReplacementFailed(ScriptableVariable from, ScriptableVariable to)
	{
		if (debugEnabled && (debugLogAll || debugFailedVariableReplacements))
		{
			Debug.Log(string.Format("{0} (ScriptableVariableReference): Failed replacing variable {1} with {2}. Contained variable: {3}", this, from, to, Variable));
		}
	}
	protected void DebugChanged()
	{
		if (debugEnabled && (debugLogAll || debugChanges))
		{
			Debug.Log(string.Format("{0} (ScriptableVariableReference): Value changed.", this));
		}
	}
	#endregion

	#region Operators
	public static implicit operator T(ScriptableVariableReference<T, K> reference)
	{
		return reference.Value;
	}
	#endregion
}
