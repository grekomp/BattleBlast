using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DontDestroySingleton<T> : DontDestroySingletonBase where T : DontDestroySingletonBase
{

	static T _instance;
	public static T Instance {
		get {
			if (InstanceExists) return _instance;

			_instance = FindObjectOfType<T>();
			if (InstanceExists) return _instance;

			CreateNewInstance();
			return _instance;
		}
	}

	public static bool InstanceExists {
		get {
			if (_instance == null)
			{
				_instance = FindObjectOfType<T>();
			}
			return _instance != null;
		}
	}

	protected virtual void Start()
	{
		T ensureCorrectInstanceDetection = Instance;
		if (InstanceExists && Instance != this && Instance.GetComponent<DontDestroyThis>() != null)
		{
			Destroy(gameObject);
		}
		else
		{
			if (GetComponent<DontDestroyThis>() == null && Application.isPlaying)
				gameObject.AddComponent<DontDestroyThis>();
		}
	}
	protected static void CreateNewInstance()
	{
		GameObject newGO = new GameObject(typeof(T).Name);
		_instance = newGO.AddComponent<T>();
		_instance.InitNewInstance();
	}
}
