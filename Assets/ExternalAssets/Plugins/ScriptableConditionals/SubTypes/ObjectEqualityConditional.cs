using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Conditionals/Object Equality Conditional")]
public class ObjectEqualityConditional : GenericConditional {
	public ObjectEqualityConditional() {
		checkParametersCount.Value = true;
		minExpectedParametersCount.Value = 2;
		maxExpectedParametersCount.Value = -1;
	}

	protected override bool EvaluateCondition(List<ScriptableVariable> parameters) {
		object comparisonBase = parameters[0];
		foreach (var parameter in parameters) {
			if (parameter.Equals(comparisonBase) == false) return false;
		}

		return true;
	}

	protected override bool ValidateParameterTypes(List<ScriptableVariable> parameters) {
		return true;
	}
}
