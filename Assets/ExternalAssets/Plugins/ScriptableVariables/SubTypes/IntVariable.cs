using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Int")]
public class IntVariable : ScriptableVariable<int> {
    public static IntVariable New(int value = default) {
        var createdVariable = CreateInstance<IntVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
