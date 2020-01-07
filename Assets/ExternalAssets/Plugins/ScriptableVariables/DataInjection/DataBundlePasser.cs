using UnityEngine;
using System.Collections;

/// <summary>
/// Takes the output of a DataSource and injects it into a DataInjector
/// </summary>
public class DataBundlePasser : ValueSetter {
	[Header("Source")]
	public DataSource source;

	[Header("Target")]
	public DataInjector target;

	protected override void ApplySet() {
		if (source && target) {
			target.Inject(source.OutputSingle);
		}
	}

	protected override void Init() {
		if (source == null) source = GetComponent<DataSource>();
		if (target == null) Debug.LogWarning(string.Format("{0} (DataBundlePasser): Target is null.", this), this);
	}
	protected override void InitValueChangedHandlers() {
		base.InitValueChangedHandlers();

		if (source)
			source.OnOutputUpdated += HandleValueChanged;
	}
}
