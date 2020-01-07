using System;
using UnityEngine;
[Serializable]
public class DataInjectorReference : ScriptableVariableReference<DataInjector, DataInjectorVariable> {
    public DataInjectorReference() : base() { }
    public DataInjectorReference(DataInjector value) : base(value) { }
    public DataInjectorReference(DataInjectorVariable variable) : base(variable) { }
}
