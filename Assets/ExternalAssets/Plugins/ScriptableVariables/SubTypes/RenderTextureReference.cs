using System;
using UnityEngine;
[Serializable]
public class RenderTextureReference : ScriptableVariableReference<RenderTexture, RenderTextureVariable> {
    public RenderTextureReference() : base() { }
    public RenderTextureReference(RenderTexture value) : base(value) { }
    public RenderTextureReference(RenderTextureVariable variable) : base(variable) { }
}
