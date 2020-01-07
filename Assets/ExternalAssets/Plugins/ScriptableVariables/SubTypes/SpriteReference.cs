using System;
using UnityEngine;
[Serializable]
public class SpriteReference : ScriptableVariableReference<Sprite, SpriteVariable> {
    public SpriteReference() : base() { }
    public SpriteReference(Sprite value) : base(value) { }
    public SpriteReference(SpriteVariable variable) : base(variable) { }
}
