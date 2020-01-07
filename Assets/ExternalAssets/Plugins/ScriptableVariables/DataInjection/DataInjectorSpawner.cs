using UnityEngine;
using System.Collections;

public class DataInjectorSpawner : ValueSetter {
	[Header("Data")]
	public BoolReference attemptToUseDataSource = new BoolReference(true);
	public DataSource dataSource;
	[HandleChanges] public DataBundleReference dataBundle;

	[Header("Prefab")]
	[HandleChanges] public DataInjectorReference childPrefab;

	[Header("Spawned Child")]
	[Disabled]
	public DataInjector spawnedChild;

	#region Initialization
	protected override void Init() {
		if (attemptToUseDataSource) {
			if (dataSource == null) dataSource = GetComponent<DataSource>();

			if (dataSource) {
				Inject(dataSource.OutputSingle);
				dataSource.OnOutputUpdated += Set;
			}
		}
	}
	#endregion

	#region Updating children
	public void Inject(DataBundle dataBundle) {
		this.dataBundle.Value = dataBundle;

		// If SetOnValueChanged is true, this will be called automatically
		if (SetOnValueChanged == false) Set();
	}

	protected override void ApplySet() {
		if (childPrefab.Value == null || dataBundle.Value == null) return;

		if (spawnedChild == null) spawnedChild = Instantiate(childPrefab.Value, transform);
		spawnedChild.name = string.Format("{0} ({1})", childPrefab.Value.name, dataBundle.Value.id);

		spawnedChild.Inject(dataBundle);
	}
	#endregion
}
