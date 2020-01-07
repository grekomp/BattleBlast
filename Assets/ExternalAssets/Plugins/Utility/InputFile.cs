using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class InputFile {

	static Dictionary<string, InputFileCachedData> cachedData = new Dictionary<string, InputFileCachedData>();

	public static bool TryGet<T>(string key, T fallback, out T retVal, string filePath = "config.txt") where T : IConvertible {
		T temp;
		if (TryGet<T>(key, out temp)) {
			retVal = temp;
			return true;
		}
		else {
			retVal = fallback;
			return false;
		}
	}

	public static bool TryGet<T>(string key, out T retVal, string filePath = "config.txt") where T : IConvertible {
		InputFileCachedData cachedFile;
		if (cachedData.ContainsKey(filePath)) {
			cachedFile = cachedData[filePath];
			if (cachedFile.HasKeyForType<T>(key)) {
				retVal = cachedFile.GetValueOfType<T>(key);
				return true;
			}
		}
		else {
			cachedFile = new InputFileCachedData();
			cachedData.Add(filePath, cachedFile);
		}
		string line = "";
		if (TryGetString(filePath, key, out line)) {
			retVal = (T)Convert.ChangeType(line, typeof(T));
			cachedFile.SetValue<T>(key, retVal);
			return true;
		}
		else {
			retVal = default(T);
			return false;
		}
	}

	public static T Get<T>(string key, T fallback, string filePath = "config.txt") where T : IConvertible {
		T temp = default(T);
		if (TryGet<T>(key, out temp, filePath)) {
			return temp;
		}
		else {
			return fallback;
		}
	}

	public static T Get<T>(string key, string filePath = "config.txt") where T : IConvertible {
		T temp = default(T);
		TryGet<T>(key, out temp, filePath);
		return temp;
	}

	static bool TryGetString(string filePath, string key, out string retVal) {
		try {
			string text = File.ReadAllText(filePath);
			text = text.Replace("\r", "");
			string[] lines = text.Split(new char[] { '\n' });
			int i = 0;
			foreach (string s in lines) {
				string lineKey = s.Trim();
				if (lineKey.CompareTo(key) == 0) {
					retVal = lines[i + 1];
					return true;
				}
				i++;
			}
			Debug.LogWarning("Couldn't load key >>" + key + "<< from file >>" + filePath + "<<!");
		}
		catch (System.Exception e) {
			Debug.LogError(e.Message);
		}
		retVal = default(string);
		return false;
	}

	class InputFileCachedData {
		List<Values> values;

		public InputFileCachedData() {
			values = new List<Values>();
		}

		public bool HasKeyForType<T>(string key) where T : IConvertible {
			Values<T> temp = GetDictionaryForType<T>();
			if (temp == null) {
				return false;
			}
			return temp.HasKey(key);
		}

		public T GetValueOfType<T>(string key) where T : IConvertible {
			Values<T> temp = GetDictionaryForType<T>();
			return temp[key];
		}

		public void SetValue<T>(string key, T val) where T : IConvertible {
			Values<T> temp = GetDictionaryForType<T>();
			if (temp == null) {
				temp = new Values<T>();
				values.Add(temp);
			}
			temp[key] = val;
		}

		Values<T> GetDictionaryForType<T>() where T : IConvertible {
			return (Values<T>)values.Find(x => x.type == typeof(T));
		}
	}

	abstract class Values {
		public abstract System.Type type { get; set; }
		public abstract bool HasKey(string key);
	}

	class Values<T> : Values where T : IConvertible {
		Dictionary<string, T> values;

		public override Type type {
			get {
				return typeof(T);
			}
			set { }
		}

		public Values() {
			values = new Dictionary<string, T>();
		}

		public override bool HasKey(string key) {
			return values.ContainsKey(key);
		}

		public T this[string key] {
			get {
				return values[key];
			}
			set {
				values[key] = value;
			}
		}
	}
}