using System;
using UnityEngine;
[Serializable]
public class BoolReference : ScriptableVariableReference<bool, BoolVariable> {
    public BoolReference() : base() { }
    public BoolReference(bool value) : base(value) { }
    public BoolReference(BoolVariable variable) : base(variable) { }
}
