using UnityEngine;
using System.Collections;
using System;

public abstract class ValueSyncer : ValueSetter {
	#region Settings
	public ValueSyncerSettings syncerSettings;

	public bool setValue => syncerSettings != null ? syncerSettings.setValue : false;
	public bool getValue => syncerSettings != null ? syncerSettings.getValue : false;
	#endregion

	public override void Set() {
		if (setValue) {
			base.Set();
		}
	}
	public virtual void Get() {
		if (syncerSettings.getValue) {
			ApplyGet();
		}
	}
	protected abstract void ApplyGet();

	#region Handling input events
	protected virtual void BindChangeEvents() { }
	protected virtual void UnbindChangeEvents() { }
	protected override void OnEnable() {
		base.OnEnable();
		BindChangeEvents();
	}
	protected void OnDisable() {
		UnbindChangeEvents();
	}
	#endregion
}
