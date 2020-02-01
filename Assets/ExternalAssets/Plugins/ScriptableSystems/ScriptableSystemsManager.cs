using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace ScriptableSystems
{
	[CreateAssetMenu(menuName = "Systems/ScriptableSystemsManager")]
	public class ScriptableSystemsManager : ScriptableObject
	{
		public ScriptableSystemCollection currentSystemsCollection;

		protected static ScriptableSystemsManager _instance;
		public static ScriptableSystemsManager Instance {
			get {
				Initialize();

				return _instance;
			}
		}

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
#else
		[RuntimeInitializeOnLoadMethod]
#endif
		public static void Initialize()
		{
			if (_instance == null)
			{
				// Initializing the ScriptableSystemsManager
				var found = Resources.LoadAll<ScriptableSystemsManager>("Systems");
				_instance = found.FirstOrDefault();
#if UNITY_EDITOR
				_instance?.Awake();
#endif
			}
			if (_instance == null)
			{
				Debug.LogWarning("ScriptableObjectSingleton: Failed to find a ScriptableObject asset of type ScriptableSystemsManager, creating a temporary runtime-only instance.");
				_instance = CreateInstance<ScriptableSystemsManager>();
#if UNITY_EDITOR
				_instance?.Awake();
#endif
			}

		}

		public T Get<T>() where T : ScriptableSystem
		{
			return currentSystemsCollection?.Get<T>();
		}

		public void Awake()
		{
			currentSystemsCollection?.Awake();
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;

#if UNITY_EDITOR
			EditorSceneManager.sceneOpened -= OnEditorSceneOpened;
			EditorSceneManager.sceneOpened += OnEditorSceneOpened;
			EditorSceneManager.sceneUnloaded -= OnSceneUnloaded;
			EditorSceneManager.sceneUnloaded += OnSceneUnloaded;
#endif
		}
		public void Start()
		{
			currentSystemsCollection?.Start();
		}

		public void OnSceneUnloaded(Scene scene)
		{
			currentSystemsCollection?.OnSceneUnloaded(scene);
		}
#if UNITY_EDITOR
		public void OnEditorSceneOpened(Scene scene, OpenSceneMode openSceneMode)
		{
			switch (openSceneMode)
			{
				case OpenSceneMode.Single:
					OnSceneLoaded(scene, LoadSceneMode.Single);
					break;
				case OpenSceneMode.Additive:
				case OpenSceneMode.AdditiveWithoutLoading:
					OnSceneLoaded(scene, LoadSceneMode.Additive);
					break;
			}
		}
#endif
		public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			currentSystemsCollection?.OnSceneLoaded(scene, loadSceneMode);
		}

		public void Update()
		{
			currentSystemsCollection?.Update();
		}
		public void FixedUpdate()
		{
			currentSystemsCollection?.FixedUpdate();
		}
	}
}