using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Float")]
public class FloatVariable : ScriptableVariable<float> {
    public static FloatVariable New(float value = default) {
        var createdVariable = CreateInstance<FloatVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
