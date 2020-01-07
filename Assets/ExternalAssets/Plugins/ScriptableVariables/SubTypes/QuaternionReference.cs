using System;
using UnityEngine;
[Serializable]
public class QuaternionReference : ScriptableVariableReference<Quaternion, QuaternionVariable> {
    public QuaternionReference() : base() { }
    public QuaternionReference(Quaternion value) : base(value) { }
    public QuaternionReference(QuaternionVariable variable) : base(variable) { }
}
