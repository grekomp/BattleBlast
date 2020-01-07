using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/DataInjector")]
public class DataInjectorVariable : ScriptableVariable<DataInjector> {
    public static DataInjectorVariable New(DataInjector value = default) {
        var createdVariable = CreateInstance<DataInjectorVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
