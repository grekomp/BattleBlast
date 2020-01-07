using System;
using UnityEngine;
[Serializable]
public class StringReference : ScriptableVariableReference<string, StringVariable> {
    public StringReference() : base() { }
    public StringReference(string value) : base(value) { }
    public StringReference(StringVariable variable) : base(variable) { }
}
