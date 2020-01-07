using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class IntFormatSetter : ValueSetter {
	public enum IntFormatType {
		Numeric,
		LatinABCDUppercase,
		LatinABCDLowercase,
		Roman
	}

	[Header("Components")]
	public TextSetter outputTextSetter;

	[Header("Variables")]
	public StringReference output;
	[Space]
	[HandleChanges] public StringReference formatString;
	[HandleChanges] public ScriptableVariableControlledSetReference values;
	public IntFormatType formatType = IntFormatType.Numeric;

	protected override void ApplySet() {
		try {
			object[] valueObjects = values.Value.Select(v => v?.ValueObject).ToArray();
			valueObjects = ConvertIntsFormat(valueObjects);

			output.Value = string.Format(new NumbersAsDateTimeFormat(), formatString, valueObjects);
			if (outputTextSetter) outputTextSetter.value.Value = output;
		}
		catch (FormatException e) {
			Debug.LogWarning(e.Message, this);
		}
	}

	private object[] ConvertIntsFormat(object[] valueObjects) {
		valueObjects = valueObjects.Select(v => {
			if (v is int intValue) {
				switch (formatType) {
					case IntFormatType.Numeric:
						return intValue;
					case IntFormatType.LatinABCDUppercase:
						return intValue.ToLatinUppercase();
					case IntFormatType.LatinABCDLowercase:
						return intValue.ToLatinLowercase();
					case IntFormatType.Roman:
						return intValue.ToRoman();
				}
			}

			return v;
		}).ToArray();
		return valueObjects;
	}

	protected override void Init() { }

	protected override void InitValueChangedHandlers() {
		base.InitValueChangedHandlers();
		output.OnVariableChanged += v => HandleValueChanged();
	}
}
