using System;
using UnityEngine;
[Serializable]
public class IntReference : ScriptableVariableReference<int, IntVariable> {
    public IntReference() : base() { }
    public IntReference(int value) : base(value) { }
    public IntReference(IntVariable variable) : base(variable) { }
}
