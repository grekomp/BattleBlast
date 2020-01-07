using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public abstract class ValueSetter : DataInjectionReceiver
{
	#region Settings
	[Header("Settings")]
	public ValueSetterSettings settings = new ValueSetterSettings();

	public bool SetOnAwake => settings.setOnAwake;
	public bool SetOnStart => settings.setOnStart;
	public bool SetOnEnable => settings.setOnEnable;
	public bool SetOnUpdate => settings.setOnUpdate;
	public bool SetOnLateUpdate => settings.setOnLateUpdate;
	public bool SetOnValidate => settings.setOnValidate;
	public bool SetOnReplaceVariable => settings.setOnReplaceVariable;
	public bool SetOnValueChanged => settings.setOnValueChanged;
	public bool SetInEditMode => settings.setInEditMode;
	#endregion

	protected bool initialized = false;

	protected virtual void Awake()
	{
		Init();
		InitValueChangedHandlers();
		initialized = true;
		if (SetOnAwake) Set();
	}
	protected virtual void Start()
	{
		if (SetOnStart) Set();
	}
	protected virtual void OnEnable()
	{
		if (SetOnEnable) Set();
	}
	protected virtual void Update()
	{
		if (SetOnUpdate) Set();
	}
	protected virtual void LateUpdate()
	{
		if (SetOnLateUpdate) Set();
	}
	protected virtual void OnValidate()
	{
		if (SetOnValidate) Set();
	}

	[ContextMenu("Set")]
	public virtual void Set()
	{
		if (SetInEditMode || EditorPlaystateHelper.cachedIsPlaying)
		{
			if (settings.debugEnabled) Debug.Log(string.Format("{0} (ValueSetter): Set.", this), this);

			try
			{
				ApplySet();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				throw;
			}
		}
	}
	/// <summary>
	/// Executes the primary action of the class
	/// </summary>
	protected abstract void ApplySet();

	/// <summary>
	/// A method used for initializing components and events
	/// </summary>
	protected abstract void Init();

	/// <summary>
	/// Bind all variables that should cause the component to update
	/// </summary>
	protected virtual void InitValueChangedHandlers()
	{
		FieldInfo[] fieldInfos = GetType().GetFields();

		List<ScriptableVariableReference> variablesToWatch = new List<ScriptableVariableReference>();
		foreach (FieldInfo fieldInfo in fieldInfos)
		{
			if (typeof(ScriptableVariableReference).IsAssignableFrom(fieldInfo.FieldType))
			{
				if (fieldInfo.GetCustomAttribute<HandleChangesAttribute>() != null)
				{
					variablesToWatch.Add(fieldInfo.GetValue(this) as ScriptableVariableReference);
				}
			}
		}

		foreach (var variableReference in variablesToWatch)
		{
			variableReference.OnChanged += HandleValueChanged;
		}
	}

	/// <summary>
	/// Should be bound to field's OnChanged events
	/// </summary>
	public virtual void HandleValueChanged()
	{
		if (SetOnValueChanged)
			Set();
	}

	#region Variable Replacement
	public override bool ReplaceVariable(ScriptableVariable from, ScriptableVariable to)
	{
		bool variableReplaced = base.ReplaceVariable(from, to);

		if (variableReplaced && SetOnReplaceVariable) Set();

		return variableReplaced;
	}
	public override bool ReplaceEvent(GameEvent from, GameEvent to)
	{
		bool eventReplaced = base.ReplaceEvent(from, to);

		if (eventReplaced && SetOnReplaceVariable) Set();

		return eventReplaced;
	}
	#endregion
}
