using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Vector3InputSyncer : ValueSyncer
{
	[Header("Variables")]
	[HandleChanges] public Vector3Reference value;

	[Header("Components")]
	public InputFieldTextSyncerDouble xInput;
	public InputFieldTextSyncerDouble yInput;
	public InputFieldTextSyncerDouble zInput;

	[Header("Events")]
	public GameEventHandler OnValueChanged;
	public GameEventHandler OnEndEdit;

	protected override void ApplyGet()
	{
		float x = (float)xInput.doubleValue.Value;
		float y = (float)yInput.doubleValue.Value;
		float z = (float)zInput.doubleValue.Value;

		value.Value = new Vector3(x, y, z);
	}

	protected override void ApplySet()
	{
		UnbindChangeEvents();

		if ((float)xInput.doubleValue.Value != value.Value.x) xInput.doubleValue.Value = value.Value.x;
		if ((float)yInput.doubleValue.Value != value.Value.y) yInput.doubleValue.Value = value.Value.y;
		if ((float)zInput.doubleValue.Value != value.Value.z) zInput.doubleValue.Value = value.Value.z;

		BindChangeEvents();
	}

	protected override void Init() { }

	#region Handling input events
	protected override void BindChangeEvents()
	{
		xInput.OnValueChanged.RegisterListenerOnce(HandleInputChanged);
		yInput.OnValueChanged.RegisterListenerOnce(HandleInputChanged);
		zInput.OnValueChanged.RegisterListenerOnce(HandleInputChanged);
		xInput.OnEndEdit.RegisterListenerOnce(HandleEndEdit);
		xInput.OnEndEdit.RegisterListenerOnce(HandleEndEdit);
		xInput.OnEndEdit.RegisterListenerOnce(HandleEndEdit);
	}
	protected override void UnbindChangeEvents()
	{
		xInput.OnValueChanged.DeregisterListener(HandleInputChanged);
		yInput.OnValueChanged.DeregisterListener(HandleInputChanged);
		zInput.OnValueChanged.DeregisterListener(HandleInputChanged);
		xInput.OnEndEdit.DeregisterListener(HandleEndEdit);
		xInput.OnEndEdit.DeregisterListener(HandleEndEdit);
		xInput.OnEndEdit.DeregisterListener(HandleEndEdit);
	}

	protected void HandleInputChanged()
	{
		Get();
		OnValueChanged?.Raise(this, value.Value);
	}
	protected void HandleEndEdit()
	{
		HandleInputChanged();
		OnEndEdit?.Raise(this, value.Value);
	}
	#endregion
}
