using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Quaternion")]
public class QuaternionVariable : ScriptableVariable<Quaternion> {
    public static QuaternionVariable New(Quaternion value = default) {
        var createdVariable = CreateInstance<QuaternionVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
