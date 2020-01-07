using System;
using UnityEngine;
[Serializable]
public class TextureReference : ScriptableVariableReference<Texture, TextureVariable> {
    public TextureReference() : base() { }
    public TextureReference(Texture value) : base(value) { }
    public TextureReference(TextureVariable variable) : base(variable) { }
}
