using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/Sprite")]
public class SpriteVariable : ScriptableVariable<Sprite> {
    public static SpriteVariable New(Sprite value = default) {
        var createdVariable = CreateInstance<SpriteVariable>();
        createdVariable.Value = value;
        return createdVariable;
    }
}
