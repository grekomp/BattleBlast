using System;
using UnityEngine;
[Serializable]
public class Vector2Reference : ScriptableVariableReference<Vector2, Vector2Variable> {
    public Vector2Reference() : base() { }
    public Vector2Reference(Vector2 value) : base(value) { }
    public Vector2Reference(Vector2Variable variable) : base(variable) { }
}
