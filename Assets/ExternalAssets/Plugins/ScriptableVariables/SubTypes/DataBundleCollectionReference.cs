using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class DataBundleCollectionReference : ScriptableVariableReference<DataBundleCollection, DataBundleCollectionVariable> {
	public DataBundleCollectionReference() : base() { }
	public DataBundleCollectionReference(DataBundleCollection value) : base(value) { }
	public DataBundleCollectionReference(DataBundleCollectionVariable variable) : base(variable) { }
}
