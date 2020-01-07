using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Conditionals/Bool Operator Conditional")]
public class BooleanOperatorConditional : GenericConditional {
	public enum OperatorType {
		And,
		Or
	}

	[Header("Options")]
	public OperatorType operatorType;

	protected override bool EvaluateCondition(List<ScriptableVariable> parameters) {
		return EvaluateOperator(parameters.Where(p => p.ValueObject is bool).Select(p => (bool)p.ValueObject).ToList());
	}
	protected bool EvaluateOperator(List<bool> parameters) {
		switch (operatorType) {
			case OperatorType.And:
				foreach (var parameter in parameters) {
					if (parameter == false) return false;
				}
				return true;
			case OperatorType.Or:
				foreach (var parameter in parameters) {
					if (parameter) return true;
				}
				return false;
		}

		return false;
	}

	protected override bool ValidateParameterTypes(List<ScriptableVariable> parameters) {
		foreach (var parameter in parameters) {
			if (parameter.ValueObject is bool == false) return false;
		}

		return true;
	}
}
