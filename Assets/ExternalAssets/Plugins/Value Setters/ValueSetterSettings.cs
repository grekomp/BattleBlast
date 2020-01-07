using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ValueSetterSettings
{
	public BoolReference setOnAwake = new BoolReference(true);
	public BoolReference setOnStart;
	public BoolReference setOnEnable;
	public BoolReference setOnUpdate;
	public BoolReference setOnLateUpdate;
	[Space]
	public BoolReference setOnValidate = new BoolReference(true);
	public BoolReference setOnReplaceVariable = new BoolReference(true);
	public BoolReference setOnValueChanged = new BoolReference(true);
	[Space]
	public BoolReference setInEditMode = new BoolReference(false);
	[Space]
	public BoolReference debugEnabled = new BoolReference(false);
}
