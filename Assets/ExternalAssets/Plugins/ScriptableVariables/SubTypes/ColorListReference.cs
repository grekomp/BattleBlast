using System;
using UnityEngine;
[Serializable]
public class ColorListReference : ScriptableVariableReference<ColorList, ColorListVariable> {
    public ColorListReference() : base() { }
    public ColorListReference(ColorList value) : base(value) { }
    public ColorListReference(ColorListVariable variable) : base(variable) { }
}
