using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : SingletonBase where T : SingletonBase {

	public static T instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<T>();
			}
			if (_instance == null && Application.isPlaying) {
				GameObject newGO = new GameObject(typeof(T).Name);
				_instance = newGO.AddComponent<T>();
				_instance.InitNewInstance();
			}
			return _instance;
		}
	}
	static T _instance;

	public static bool instanceExists {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<T>();
			}
			return _instance != null;
		}
	}

	protected virtual void Start() {
		if (instanceExists && instance != this) {
			Destroy(gameObject);
		}
		else {
			_instance = this as T;
		}
	}
}
