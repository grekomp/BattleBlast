using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// A wrapper for generic conditionals that allows you to define constant parameters to be used during condition checking.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Conditionals/Parametrized Conditional")]
public class ParametrizedConditional : ScriptableConditional {

	[Header("Constant Parameters")]
	public ScriptableVariableControlledSetReference prependParameters;
	public ScriptableVariableControlledSetReference appendParameters;

	[Header("Conditional")]
	public GenericConditional underlyingConditional;

	public override bool CheckCondition(List<ScriptableVariable> parameters) {
		List<ScriptableVariable> mergedParametersList = new List<ScriptableVariable>(prependParameters.Value);
		mergedParametersList.AddRange(parameters);
		mergedParametersList.AddRange(appendParameters.Value);

		return underlyingConditional.CheckCondition(mergedParametersList);
	}
}
