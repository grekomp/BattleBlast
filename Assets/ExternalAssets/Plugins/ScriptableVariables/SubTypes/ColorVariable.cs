using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Color")]
public class ColorVariable : ScriptableVariable<Color> {
    public static ColorVariable New(Color value = default) {
        var createdVariable = CreateInstance<ColorVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
