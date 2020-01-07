using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/RenderTexture")]
public class RenderTextureVariable : ScriptableVariable<RenderTexture> {
    public static RenderTextureVariable New(RenderTexture value = default) {
        var createdVariable = CreateInstance<RenderTextureVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
