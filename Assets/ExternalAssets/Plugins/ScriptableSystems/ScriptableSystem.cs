using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

namespace ScriptableSystems
{
	public class ScriptableSystem : ScriptableObject
	{
		[Header("Scriptable System Options")]
		public bool initializeOnAwake = true;
		[NonSerialized]
		protected bool initialized = false;
		public bool IsInitialized {
			get { return initialized; }
		}

		public virtual void Awake() { if (initializeOnAwake) Initialize(); }
		public virtual void Update() { }
		public virtual void FixedUpdate() { }
		public virtual void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) { }
		public virtual void OnSceneUnloaded(Scene scene) { }

		public virtual void Initialize()
		{
			if (initialized == false) OnInitialize();
		}
		[ContextMenu("Reinitialize")]
		protected virtual void OnInitialize()
		{
			// Prevents systems from being initialized when the editor is switching into, or out of play mode
			if (EditorPlaystateHelper.IsEditorEnteringPlay == false && EditorPlaystateHelper.IsEditorQuittingPlay == false)
			{
				initialized = true;
			}
		}

	}

	public class ScriptableSystem<T> : ScriptableSystem where T : ScriptableSystem
	{
		public static T Instance {
			get {
				T instance = ScriptableSystemsManager.Instance.Get<T>();
				instance?.Initialize();
				return instance;
			}
		}
	}
}