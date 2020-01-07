using System;
using UnityEngine;
[Serializable]
public class DataBundleReference : ScriptableVariableReference<DataBundle, DataBundleVariable> {
    public DataBundleReference() : base() { }
    public DataBundleReference(DataBundle value) : base(value) { }
    public DataBundleReference(DataBundleVariable variable) : base(variable) { }
}
