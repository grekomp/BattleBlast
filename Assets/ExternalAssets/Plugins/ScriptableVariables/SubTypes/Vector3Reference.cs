using System;
using UnityEngine;
[Serializable]
public class Vector3Reference : ScriptableVariableReference<Vector3, Vector3Variable> {
    public Vector3Reference() : base() { }
    public Vector3Reference(Vector3 value) : base(value) { }
    public Vector3Reference(Vector3Variable variable) : base(variable) { }
}
