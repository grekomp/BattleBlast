using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/ScriptableVariableControlledSet")]
public class ScriptableVariableControlledSetVariable : ScriptableVariable<ScriptableVariableControlledSet> {
    public static ScriptableVariableControlledSetVariable New(ScriptableVariableControlledSet value = default) {
        var createdVariable = CreateInstance<ScriptableVariableControlledSetVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
