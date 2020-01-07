using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A generic conditional that accepts a collection of input variables and does not have any predefined parameters.
/// 
/// Used inside parametralized conditionals to define the underlying functionality.
/// </summary>
public abstract class GenericConditional : ScriptableConditional {

	[Header("Options")]
	public BoolReference checkParametersCount = new BoolReference(true);
	public IntReference minExpectedParametersCount = new IntReference(2);
	[Tooltip("Use -1 for infinite maximum parameters")]
	public IntReference maxExpectedParametersCount = new IntReference(2);

	public override bool CheckCondition(List<ScriptableVariable> parameters) {
		if (ValidateInputs(parameters) == false) return false;

		return EvaluateCondition(parameters);
	}
	protected abstract bool EvaluateCondition(List<ScriptableVariable> parameters);

	public virtual bool ValidateInputs(List<ScriptableVariable> parameters) {
		return ValidateParametersCount(parameters) && ValidateParameterTypes(parameters);
	}
	protected abstract bool ValidateParameterTypes(List<ScriptableVariable> parameters);
	protected bool ValidateParametersCount(List<ScriptableVariable> parameters) {
		if (checkParametersCount == false) return true;
		if (parameters.Count >= minExpectedParametersCount && (parameters.Count <= maxExpectedParametersCount || maxExpectedParametersCount == -1)) return true;

		return false;
	}

	private void OnValidate() {
		// Validate options
		if (minExpectedParametersCount < 0) minExpectedParametersCount.Value = 0;
		if (maxExpectedParametersCount != -1 && maxExpectedParametersCount < minExpectedParametersCount) maxExpectedParametersCount.Value = minExpectedParametersCount;
	}
}
