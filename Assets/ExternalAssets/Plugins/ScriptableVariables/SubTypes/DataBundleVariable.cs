using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/DataBundle")]
public class DataBundleVariable : ScriptableVariable<DataBundle> {
    public static DataBundleVariable New(DataBundle value = default) {
        var createdVariable = CreateInstance<DataBundleVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
