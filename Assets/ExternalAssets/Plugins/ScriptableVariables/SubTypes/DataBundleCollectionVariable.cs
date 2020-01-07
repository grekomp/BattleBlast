using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Variables/DataBundleCollection")]
public class DataBundleCollectionVariable : ScriptableVariable<DataBundleCollection> {
	public static DataBundleCollectionVariable New(DataBundleCollection value = default) {
		var createdVariable = CreateInstance<DataBundleCollectionVariable>();
		createdVariable.Value = value;
		return createdVariable;
	}
}
