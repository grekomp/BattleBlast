using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class DataKey : ISerializationCallbackReceiver {
	public ScriptableObject variable;
	public string keyString;

	[NonSerialized]
	protected string key = null;

	public DataKey() { }
	public DataKey(string key) => keyString = key;

	public string GetKeyString() {
		CreateKey();
		return key;
	}

	private void CreateKey() {
		if (string.IsNullOrEmpty(key)) {
			if (variable) {
				key = GetKeyFromName(variable.name);
			}
			else {
				key = keyString;
			}
		}
	}

	public override bool Equals(object obj) {
		if (obj is DataKey dataKey) return Equals(dataKey);

		return base.Equals(obj);
	}
	public bool Equals(DataKey otherDataKey) {
		return otherDataKey.GetKeyString() == GetKeyString();
	}
	public override int GetHashCode() {
		var hashCode = -72669015;
		hashCode = hashCode * -1521134295 + EqualityComparer<ScriptableObject>.Default.GetHashCode(variable);
		hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(keyString);
		return hashCode;
	}

	protected string GetKeyFromName(string name) {
		string[] splitName = name.Split(' ');

		StringBuilder sb = new StringBuilder();

		if (splitName[0] != "_r") {
			sb.Append(splitName[0]);
			if (splitName.Count() > 1) {
				sb.Append("_");
			}
		}

		for (int i = 1; i < splitName.Count(); i++) {
			if (i > 1) sb.Append("_");

			sb.Append(splitName[i]);
		}

		return sb.ToString();
	}

	public void OnBeforeSerialize() {
		key = null;
	}
	public void OnAfterDeserialize() {
		key = null;
	}

	public override string ToString() {
		return GetKeyString();
	}
	public static implicit operator string(DataKey dataKey) {
		return dataKey.GetKeyString();
	}
}
