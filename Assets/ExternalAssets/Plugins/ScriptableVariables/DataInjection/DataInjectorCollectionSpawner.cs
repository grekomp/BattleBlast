using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataInjectorCollectionSpawner : DataInjectionReceiver {
	[Serializable]
	public class SpawnedInjector {
		public DataBundle dataBundle;
		public DataInjector injector;
	}

	[Header("Data")]
	public BoolReference attemptToUseDataSource = new BoolReference(true);
	public DataSource dataSource;
	public DataBundleCollectionReference dataBundleCollection;

	[Header("Prefab")]
	public DataInjectorReference childPrefab;

	[Header("Spawned Children")]
	[SerializeField]
	[Disabled]
	List<SpawnedInjector> spawnedChildren = new List<SpawnedInjector>();
	[SerializeField]
	[Disabled]
	List<DataInjector> pooledObjects = new List<DataInjector>();
	[SerializeField]
	[Disabled]
	List<DataInjector> objectsReturningToPool = new List<DataInjector>();

	[Header("Options")]
	public BoolReference enableObjectPooling;
	public BoolReference logUpdates;
	public DataKey autonumberingKey = new DataKey("_Index");

	#region Initialization
	private void Awake() {
		if (attemptToUseDataSource) {
			if (dataSource == null) dataSource = GetComponent<DataSource>();

			if (dataSource) {
				Inject(dataSource.output);
				dataSource.output.OnValueChanged += Inject;
			}
			else {
				dataBundleCollection.OnChanged += UpdateChildren;
			}
		}

		childPrefab.OnChanged += HandleChildPrefabChanged;
	}
	#endregion

	#region Updating children
	public void Inject(DataBundleCollection dataBundles) {
		dataBundleCollection.Value = dataBundles;
		UpdateChildren();
	}

	[ContextMenu("Update Children")]
	public void UpdateChildren() {
		if (logUpdates) Debug.Log("DataInjectorCollection: Updating Children", this);

		RemoveEmptyDataBundles();
		UpdateDataBundlesIndex();
		RemoveAdditionalChildren();
		AddMissingChildren();
		UpdateCurrentChildren();
	}

	private void RemoveEmptyDataBundles() {
		for (int i = dataBundleCollection.Value.dataBundles.Count - 1; i >= 0; i--) {
			if (dataBundleCollection.Value.dataBundles[i] == null) {
				if (logUpdates) Debug.Log("DataInjectorCollection: Removing empty dataBundle at " + i, this);
				dataBundleCollection.Value.dataBundles.RemoveAt(i);
			}
		}
	}
	private void UpdateDataBundlesIndex() {
		for (int i = 0; i < dataBundleCollection.Value.dataBundles.Count; i++) {
			dataBundleCollection.Value.dataBundles[i].SetOrUpdateValue("_Index", IntVariable.New(i));
		}
	}
	private void UpdateCurrentChildren() {
		int i = 0;
		foreach (DataBundle dataBundle in dataBundleCollection.Value.dataBundles) {
			SpawnedInjector spawnedInjector = GetSpawnedInjector(dataBundle);

			UpdateChildDataBundle(spawnedInjector, dataBundle);
			UpdateChildSiblingIndex(spawnedInjector, i);
			i++;
		}
	}

	private void AddMissingChildren() {
		int i = 0;
		foreach (DataBundle dataBundle in dataBundleCollection.Value.dataBundles) {
			if (spawnedChildren.Find(s => s.dataBundle.id == dataBundle.id) == null) {
				AddChild(dataBundle, i);
			}
			i++;
		}
	}
	private void AddChild(DataBundle dataBundle, int siblingIndex = -1) {
		if (logUpdates) Debug.Log("DataInjectorCollection: Adding child for id: " + dataBundle.id, this);
		if (childPrefab.Value == null) {
			Debug.LogError("DataInjectorCollection: Cannot add child because the prefab is null.", this);
			return;
		}

		DataInjector child;
		if (enableObjectPooling) {
			child = GetFromPool();
		}
		else {
			child = Instantiate(childPrefab.Value, transform);
		}
		child.name = string.Format("{0} ({1})", childPrefab.Value.name, dataBundle.id);

		if (siblingIndex >= 0)
			child.transform.SetSiblingIndex(siblingIndex);

		child.Inject(dataBundle);

		spawnedChildren.Add(new SpawnedInjector() {
			dataBundle = dataBundle,
			injector = child
		});
	}

	[ContextMenu("Clear Spawned Children")]
	private void ClearChildren() {
		for (int i = spawnedChildren.Count - 1; i >= 0; i--) {
			RemoveChild(spawnedChildren[i]);
		}

		ClearPool();
	}
	private void RemoveAdditionalChildren() {
		for (int i = spawnedChildren.Count - 1; i >= 0; i--) {
			if (dataBundleCollection.Value.dataBundles.Find(d => d.id == spawnedChildren[i].dataBundle.id) == null) {
				RemoveChild(spawnedChildren[i]);
			}
		}
	}
	private void RemoveChild(SpawnedInjector spawnedInjector) {
		if (logUpdates) Debug.Log("DataInjectorCollection: Removing child: " + spawnedInjector.injector, this);

		if (enableObjectPooling) {
			DisposeChildToPool(spawnedInjector.injector);
		}
		else {
			spawnedInjector.injector?.DisposeAndDestroy();
		}

		spawnedChildren.Remove(spawnedInjector);
	}

	private void HandleChildPrefabChanged() {
		ClearChildren();
		UpdateChildren();
	}
	#endregion

	#region ObjectPooling
	protected DataInjector GetFromPool() {
		DataInjector toReturn = null;
		if (pooledObjects.Count > 0) {
			toReturn = pooledObjects[0];
			pooledObjects.RemoveAt(0);
			toReturn.gameObject.SetActive(true);
		}
		else {
			toReturn = Instantiate(childPrefab.Value, transform);
		}

		return toReturn;
	}
	protected void DisposeChildToPool(DataInjector dataInjector) {
		if (dataInjector == null) return;

		objectsReturningToPool.Add(dataInjector);
		dataInjector?.Dispose(() => ReturnToPool(dataInjector));
	}
	protected void ReturnToPool(DataInjector dataInjector) {
		if (objectsReturningToPool.Contains(dataInjector)) objectsReturningToPool.Remove(dataInjector);

		dataInjector.gameObject.SetActive(false);
		dataInjector.gameObject.name = string.Format("{0} (Pooled)", childPrefab.Value.name);
		pooledObjects.Add(dataInjector);
		dataInjector.transform.SetAsLastSibling();
	}
	protected void ClearPool() {
		foreach (var item in objectsReturningToPool) {
			item.CancelDispose();
			item.Destroy();
		}
		foreach (var item in pooledObjects) {
			item.Destroy();
		}

		pooledObjects.Clear();
	}
	#endregion

	#region Helpers 
	protected SpawnedInjector GetSpawnedInjector(DataBundle dataBundle) {
		foreach (SpawnedInjector spawnedInjector in spawnedChildren) {
			if (spawnedInjector.dataBundle.id == dataBundle.id) {
				return spawnedInjector;
			}
		}

		return null;
	}
	protected DataBundle GetDataBundle(string id) {
		return dataBundleCollection.Value.dataBundles.Find(d => d.id == id);
	}
	protected void UpdateChildDataBundle(SpawnedInjector spawnedInjector, DataBundle newDataBundle) {
		if (newDataBundle != spawnedInjector.dataBundle) {
			if (logUpdates) Debug.Log("DataInjectorCollection: Updating child DataBundle: " + spawnedInjector.dataBundle.id, this);

			spawnedInjector.dataBundle = newDataBundle;
			spawnedInjector.injector?.Inject(spawnedInjector.dataBundle);
		}
	}
	private void UpdateChildSiblingIndex(SpawnedInjector spawnedInjector, int siblingIndex) {
		spawnedInjector.injector.transform.SetSiblingIndex(siblingIndex);
	}
	public override bool ReplaceVariable(ScriptableVariable from, ScriptableVariable to) {
		bool variableReplaced = base.ReplaceVariable(from, to);

		if (variableReplaced) {
			UpdateChildren();
		}

		return variableReplaced;
	}
	#endregion
}
