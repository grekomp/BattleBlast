using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/String")]
public class StringVariable : ScriptableVariable<string> {
    public static StringVariable New(string value = default) {
        var createdVariable = CreateInstance<StringVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
