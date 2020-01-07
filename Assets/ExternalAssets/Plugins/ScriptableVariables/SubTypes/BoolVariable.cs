using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Bool")]
public class BoolVariable : ScriptableVariable<bool> {
    public static BoolVariable New(bool value = default) {
        var createdVariable = CreateInstance<BoolVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
