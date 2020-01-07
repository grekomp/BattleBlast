using System;
using UnityEngine;
[Serializable]
public class TransformReference : ScriptableVariableReference<Transform, TransformVariable> {
    public TransformReference() : base() { }
    public TransformReference(Transform value) : base(value) { }
    public TransformReference(TransformVariable variable) : base(variable) { }
}
