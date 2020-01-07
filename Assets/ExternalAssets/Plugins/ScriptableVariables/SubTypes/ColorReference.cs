using System;
using UnityEngine;
[Serializable]
public class ColorReference : ScriptableVariableReference<Color, ColorVariable> {
    public ColorReference() : base() { }
    public ColorReference(Color value) : base(value) { }
    public ColorReference(ColorVariable variable) : base(variable) { }
}
