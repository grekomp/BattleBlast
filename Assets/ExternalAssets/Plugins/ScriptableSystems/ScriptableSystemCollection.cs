using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace ScriptableSystems
{
	[CreateAssetMenu(menuName = "Systems/ScriptableSystemCollection")]
	public class ScriptableSystemCollection : ScriptableObject
	{
		public List<ScriptableSystem> systems = new List<ScriptableSystem>();

		public T Get<T>() where T : ScriptableSystem
		{
			foreach (var system in systems)
			{
				if (system is T typeSystem)
				{
					return typeSystem;
				}
			}

			Debug.LogWarning("ScriptableSystemCollection: Cannot find system of type " + typeof(T).Name);
			return null;
		}

		public void Awake()
		{
			foreach (var system in systems)
			{
				system?.Awake();
			}
		}
		public void Start()
		{
			foreach (var system in systems)
			{
				system?.Start();
			}
		}
		public void Update()
		{
			foreach (var system in systems)
			{
				system?.Update();
			}
		}
		public void FixedUpdate()
		{
			foreach (var system in systems)
			{
				system?.FixedUpdate();
			}
		}
		public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			foreach (var system in systems)
			{
				system?.OnSceneLoaded(scene, loadSceneMode);
			}
		}
		public void OnSceneUnloaded(Scene scene)
		{
			foreach (var system in systems)
			{
				system?.OnSceneUnloaded(scene);
			}
		}
	}
}