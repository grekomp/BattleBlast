using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

namespace ScriptableSystems
{
	public class ScriptableSystem : ScriptableObject
	{
		[Serializable]
		public class ScriptableSystemSettings
		{
			public bool initializeOnAwake = true;
			public bool disposeOnDisable = true;
			[Space]
			public bool playModeOnly = true;
		}

		[Header("Scriptable System Settings")]
		public ScriptableSystemSettings settings = new ScriptableSystemSettings();

		[NonSerialized]
		protected bool initialized = false;
		public bool IsInitialized {
			get { return initialized; }
		}

		#region Handling events
		public void Awake()
		{
			if (CanExecuteEvents())
			{
				if (settings.initializeOnAwake) Initialize();
				OnAwake();
			}
		}
		public void Start()
		{
			if (CanExecuteEvents())
			{
				OnStart();
			}
		}
		public void Update()
		{
			if (CanExecuteEvents()) OnUpdate();
		}
		public void FixedUpdate()
		{
			if (CanExecuteEvents()) OnFixedUpdate();
		}

		public virtual void OnDisable()
		{
			if (settings.disposeOnDisable) Dispose();
		}

		protected virtual void OnAwake() { }
		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnFixedUpdate() { }

		public virtual void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) { }
		public virtual void OnSceneUnloaded(Scene scene) { }

		protected virtual bool CanExecuteEvents()
		{
			return settings.playModeOnly == false || EditorPlaystateHelper.IsPlaying;
		}
		#endregion

		public virtual void Initialize()
		{
			if (initialized == false) OnInitialize();
		}
		[ContextMenu("Force OnInitialize")]
		protected virtual void OnInitialize()
		{
			// Prevents systems from being initialized when the editor is switching into, or out of play mode
			if (EditorPlaystateHelper.IsEditorEnteringPlay == false && EditorPlaystateHelper.IsEditorQuittingPlay == false)
			{
				initialized = true;
			}
		}

		public virtual void Dispose()
		{
			initialized = false;
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