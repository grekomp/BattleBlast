using System;
using UnityEngine;
[Serializable]
public class FloatReference : ScriptableVariableReference<float, FloatVariable> {
    public FloatReference() : base() { }
    public FloatReference(float value) : base(value) { }
    public FloatReference(FloatVariable variable) : base(variable) { }
}
