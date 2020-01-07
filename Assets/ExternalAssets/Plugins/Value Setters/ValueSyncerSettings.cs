using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ValueSyncerSettings {
	/// <summary>
	/// Whether the syncer should set the components value from variables
	/// </summary>
	public BoolReference setValue = new BoolReference(true);
	/// <summary>
	/// Whether the synced should get the variable value from components
	/// </summary>
	public BoolReference getValue = new BoolReference(true);
}
