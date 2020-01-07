using UnityEngine;
using System.Collections;
using System;

public abstract class DataSource : ValueSetter {
	[Header("Output")]
	public DataBundleCollectionReference output;

	public DataBundle OutputSingle { get { return output.Value.dataBundles.Count > 0 ? output.Value.dataBundles[0] : null; } }
	public DataBundleCollection OutputCollection { get { return output.Value; } }

	public GameEventHandler outputUpdated;
	public event Action OnOutputUpdated;

	protected override void Init() { }

	protected virtual void Output(DataBundleCollection dataBundleCollection, bool copyValues = true) {
		if (output.useConstant == true) {
			output.SetAndUseVariable(DataBundleCollectionVariable.New(dataBundleCollection));
		}

		if (copyValues) {
			output.Value.CopyValuesFrom(dataBundleCollection);
			output.HandleChange();
		}
		else {
			output.Value = dataBundleCollection;
		}

		OutputUpdated();
	}
	protected virtual void Output(DataBundle dataBundle, bool copyValues = true) {
		DataBundleCollection dataBundleCollection = new DataBundleCollection();
		dataBundleCollection.Add(dataBundle);

		Output(dataBundleCollection, copyValues);
	}

	protected void OutputUpdated() {
		outputUpdated?.Raise(this, OutputCollection);
		OnOutputUpdated?.Invoke();
	}
}
