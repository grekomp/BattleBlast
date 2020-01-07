using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class TextFormatSetter : ValueSetter {
	[Header("Components")]
	public TextSetter outputTextSetter;

	[Header("Variables")]
	public StringReference output;
	[Space]
	[HandleChanges] public StringReference formatString;
	[HandleChanges] public ScriptableVariableControlledSetReference values;

	protected override void ApplySet() {
		try {
			output.Value = string.Format(new NumbersAsDateTimeFormat(), formatString, values.Value.Select(v => v?.ValueObject).ToArray());
			if (outputTextSetter) outputTextSetter.value.Value = output;
		}
		catch (FormatException e) {
			Debug.LogWarning(e.Message, this);
		}
	}

	protected override void Init() {
		if (outputTextSetter == null) outputTextSetter = GetComponent<TextSetter>();
	}

	protected override void InitValueChangedHandlers() {
		base.InitValueChangedHandlers();
		output.OnVariableChanged += v => HandleValueChanged();
	}
}
