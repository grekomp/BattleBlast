using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Vector3")]
public class Vector3Variable : ScriptableVariable<Vector3> {
    public static Vector3Variable New(Vector3 value = default) {
        var createdVariable = CreateInstance<Vector3Variable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
