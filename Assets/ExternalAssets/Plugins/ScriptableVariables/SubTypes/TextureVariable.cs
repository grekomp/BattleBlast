using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Texture")]
public class TextureVariable : ScriptableVariable<Texture> {
    public static TextureVariable New(Texture value = default) {
        var createdVariable = CreateInstance<TextureVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
