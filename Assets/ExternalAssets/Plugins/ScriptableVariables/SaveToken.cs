using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class SaveToken {
	public string name;

	public virtual SaveToken CreateFrom(ScriptableVariable scriptableVariable) { return null; }
	public virtual void UpdateVariable(ScriptableVariable scriptableVariable) { }
}

[Serializable]
public class SaveToken<T> : SaveToken {
	public T value;
	public bool resetValueOnAwake;

	public override SaveToken CreateFrom(ScriptableVariable scriptableVariable) {
		ScriptableVariable<T> castedVariable = scriptableVariable as ScriptableVariable<T>;

		if (castedVariable) {
			name = castedVariable.name;
			value = castedVariable.Value;
			resetValueOnAwake = castedVariable.resetValueOnStartup;
		}

		return this;
	}
	public override void UpdateVariable(ScriptableVariable scriptableVariable) {
		ScriptableVariable<T> castedVariable = scriptableVariable as ScriptableVariable<T>;

		if (castedVariable) {
			castedVariable.name = name;
			castedVariable.Value = value;
			castedVariable.resetValueOnStartup = resetValueOnAwake;
		}
	}
}
