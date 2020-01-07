using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataBundle : ScriptableObject {
	public string id = Guid.NewGuid().ToString();
	public List<DataKeyPair> data = new List<DataKeyPair>();
	public List<EventKeyPair> events = new List<EventKeyPair>();

	#region Initialiazation
	public static DataBundle New() => CreateInstance<DataBundle>();
	public static DataBundle New(string id) {
		DataBundle newDataBundle = New();
		newDataBundle.id = id;
		return newDataBundle;
	}
	public static DataBundle New(UnityEngine.Object obj) => New(obj.GetInstanceID().ToString());
	public void AddFrom(DataBundle dataBundle, bool overwriteExisting = false) {
		foreach (var dataKeyPair in dataBundle.data) {
			if (overwriteExisting || GetVariable(dataKeyPair.key) == null) {
				Set(dataKeyPair.key, dataKeyPair.data);
			}
		}

		foreach (var eventKeyPair in dataBundle.events) {
			if (overwriteExisting || GetEvent(eventKeyPair.key) == null) {
				Set(eventKeyPair.key, eventKeyPair.gameEvent);
			}
		}
	}

	/// <summary>
	/// Copies the values from all variables in the provided DataBundle, without overriding the variables themselves.
	/// </summary>
	public void CopyValuesFrom(DataBundle dataBundle) {
		foreach (var dataKeyPair in dataBundle.data) {
			SetOrUpdateValue(dataKeyPair.key, dataKeyPair.data);
		}

		foreach (var eventKeyPair in dataBundle.events) {
			Set(eventKeyPair.key, eventKeyPair.gameEvent);
		}
	}
	#endregion

	#region Getting and setting data
	public T GetData<T>(string key) {
		ScriptableVariable variable = GetVariable(key);
		if (variable) {
			return (T)variable.ValueObject;
		}

		return default;
	}
	public ScriptableVariable GetVariable(string key) {
		return data.Find(d => d.key == key)?.data;
	}
	public void Set(string key, ScriptableVariableReference scriptableVariableReference) {
		Set(key, scriptableVariableReference.GetValueAsVariable());
	}
	public void Set(string key, ScriptableVariable variable) {
		DataKeyPair dataPair = data.Find(d => d.key == key);

		if (dataPair == null) {
			dataPair = new DataKeyPair() {
				key = new StringReference(key),
			};

			data.Add(dataPair);
		}

		dataPair.data = variable;
	}
	public void SetOrUpdateValue(string key, ScriptableVariable variable) {
		ScriptableVariable existingVariable = GetVariable(key);
		if (existingVariable == null) {
			Set(key, variable);
		}
		else {
			existingVariable.CopyValueFrom(variable);
		}
	}
	#endregion

	#region Events
	public GameEvent GetEvent(string key) {
		return events.Find(e => e.key == key)?.gameEvent;
	}
	public void Set(string key, GameEvent gameEvent) {
		EventKeyPair pair = events.Find(e => e.key == key);

		if (pair == null) {
			pair = new EventKeyPair() {
				key = new StringReference(key)
			};
			events.Add(pair);
		}

		pair.gameEvent = gameEvent;
	}
	#endregion
}
