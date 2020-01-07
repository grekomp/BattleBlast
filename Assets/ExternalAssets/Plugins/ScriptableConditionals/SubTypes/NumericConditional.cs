using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Conditionals/Numeric Conditional")]
public class NumericConditional : GenericConditional {
	public enum ComparisonType {
		Equal,
		NotEqual,
		Greater,
		Smaller,
		GreaterOrEqual,
		SmallerOrEqual
	}

	[Header("Options")]
	public ComparisonType comparisonType;

	protected override bool EvaluateCondition(List<ScriptableVariable> parameters) {
		switch (comparisonType) {
			case ComparisonType.Equal:
				return parameters[0].ValueObject.Equals(parameters[1].ValueObject);
			case ComparisonType.NotEqual:
				return !parameters[0].ValueObject.Equals(parameters[1].ValueObject);
			case ComparisonType.Greater:
			case ComparisonType.Smaller:
			case ComparisonType.GreaterOrEqual:
			case ComparisonType.SmallerOrEqual:
				return CompareParameters(parameters[0], parameters[1]);
		}

		return false;
	}

	private bool CompareParameters(ScriptableVariable parameter1, ScriptableVariable parameter2) {
		if (parameter1.ValueObject is IComparable comparableParameter1) {
			object valueObject = parameter2.ValueObject;

			int comparisonResult = comparableParameter1.CompareTo(valueObject);

			switch (comparisonType) {
				case ComparisonType.Greater:
					return comparisonResult > 0;
				case ComparisonType.Smaller:
					return comparisonResult < 0;
				case ComparisonType.GreaterOrEqual:
					return comparisonResult >= 0;
				case ComparisonType.SmallerOrEqual:
					return comparisonResult <= 0;
			}
		}

		return false;
	}

	protected override bool ValidateParameterTypes(List<ScriptableVariable> parameters) {
		switch (comparisonType) {
			case ComparisonType.Equal:
			case ComparisonType.NotEqual:
				return true;
			case ComparisonType.Greater:
			case ComparisonType.Smaller:
			case ComparisonType.GreaterOrEqual:
			case ComparisonType.SmallerOrEqual:
				return parameters[0].ValueObject is IComparable comparableParameter1;
		}

		return true;
	}
}

