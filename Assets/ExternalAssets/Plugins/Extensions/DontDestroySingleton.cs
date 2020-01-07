using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DontDestroySingleton<T> : DontDestroySingletonBase where T : DontDestroySingletonBase {

	static T _instance;
	public static T instance {
		get {
			if (instanceExists) return _instance;

			_instance = FindObjectOfType<T>();
			if (instanceExists) return _instance;

			CreateNewInstance();
			return _instance;
		}
	}

	public static bool instanceExists {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<T>();
			}
			return _instance != null;
		}
	}

	protected virtual void Start() {
        T ensureCorrectInstanceDetection = instance;
		if (instanceExists && instance != this && instance.GetComponent<DontDestroyThis>() != null) {
			Destroy(gameObject);
		}
		else {
			if (GetComponent<DontDestroyThis>() == null && Application.isPlaying)
				gameObject.AddComponent<DontDestroyThis>();
		}
	}
	protected static void CreateNewInstance() {
		GameObject newGO = new GameObject(typeof(T).Name);
		_instance = newGO.AddComponent<T>();
		_instance.InitNewInstance();
	}
}
