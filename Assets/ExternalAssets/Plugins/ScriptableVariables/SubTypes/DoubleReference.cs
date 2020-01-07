using System;
using UnityEngine;
[Serializable]
public class DoubleReference : ScriptableVariableReference<double, DoubleVariable> {
    public DoubleReference() : base() { }
    public DoubleReference(double value) : base(value) { }
    public DoubleReference(DoubleVariable variable) : base(variable) { }
}
