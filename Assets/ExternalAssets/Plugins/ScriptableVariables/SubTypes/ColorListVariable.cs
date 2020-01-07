using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/ColorList")]
public class ColorListVariable : ScriptableVariable<ColorList> {
    public static ColorListVariable New(ColorList value = default) {
        var createdVariable = CreateInstance<ColorListVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
