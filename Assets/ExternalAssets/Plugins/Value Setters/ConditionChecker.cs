using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ConditionChecker : ValueSetter {

	[Header("Condition")]
	public ScriptableConditional conditional;

	[Header("Data")]
	[HandleChanges] public ScriptableVariableControlledSetReference inputData;

	[Header("Output")]
	[HandleChanges] public BoolReference invertOutput;
	public BoolReference output;
	public BoolUnityEvent onOutputUpdated;

	protected override void ApplySet() {
		bool conditionResult = conditional.CheckCondition(inputData.Value);

		if (invertOutput) conditionResult = !conditionResult;

		if (output != conditionResult) {
			output.Value = conditionResult;
			onOutputUpdated.Invoke(conditionResult);
		}
	}

	protected override void Init() { }
}
