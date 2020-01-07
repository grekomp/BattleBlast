using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class EditorPlaystateHelper
{
	public static bool initialized = false;
	public static bool cachedIsPlaying = false;
#if UNITY_EDITOR
	public static PlayModeStateChange lastState = PlayModeStateChange.EnteredEditMode;
#endif
	public static bool IsPlaying {
		get {
#if UNITY_EDITOR
			return EditorApplication.isPlaying;
#else
			return true;
#endif
		}
	}
	public static bool IsEditorQuittingPlay {
		get {
#if UNITY_EDITOR
			return EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == true && Time.frameCount == 0;
#else
			return false;
#endif
		}
	}
	public static bool IsEditorEnteringPlay {
		get {
#if UNITY_EDITOR
			return EditorApplication.isPlayingOrWillChangePlaymode == true && EditorApplication.isPlaying == false;
#else
			return false;
#endif
		}
	}

	#region Runtime scene change events
	public static event UnityAction<Scene, LoadSceneMode> SceneLoaded;
	public static event UnityAction<Scene> SceneUnloaded;
	public static event UnityAction<Scene, Scene> ActiveSceneChanged;
	#endregion
	#region Editor-only scene change events
	public static event Action<Scene, string> SceneSaving;
	public static event Action<Scene> SceneSaved;
	public static event Action<Scene, bool> SceneClosing;
	public static event Action<Scene> SceneClosed;
#if UNITY_EDITOR
	public static event Action<string, OpenSceneMode> SceneOpening;
	public static event Action<Scene, OpenSceneMode> SceneOpened;
#endif
	#endregion

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Initialize()
	{
		initialized = true;
#if UNITY_EDITOR
		cachedIsPlaying = EditorApplication.isPlaying;
		EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
		EditorApplication.playModeStateChanged += HandlePlayModeStateChange;

		EditorSceneManager.sceneLoaded += HandleSceneLoaded;
		EditorSceneManager.sceneUnloaded += HandleSceneUnloaded;
		EditorSceneManager.activeSceneChanged += HandleActiveSceneChanged;
		EditorSceneManager.activeSceneChangedInEditMode += HandleActiveSceneChanged;

		EditorSceneManager.sceneSaving += HandleSceneSaving;
		EditorSceneManager.sceneSaved += HandleSceneSaved;
		EditorSceneManager.sceneClosing += HandleSceneClosing;
		EditorSceneManager.sceneClosed += HandleSceneClosed;
		EditorSceneManager.sceneOpening += HandleSceneOpening;
		EditorSceneManager.sceneOpened += HandleSceneOpened;
#else
		cachedIsPlaying = Application.isPlaying;

		SceneManager.sceneLoaded += HandleSceneLoaded;
		SceneManager.sceneUnloaded += HandleSceneUnloaded;
		SceneManager.activeSceneChanged += HandleActiveSceneChanged;
#endif
	}

	#region Event Handlers
#if UNITY_EDITOR
	private static void HandleSceneOpened(Scene scene, OpenSceneMode mode) => SceneOpened?.Invoke(scene, mode);
	private static void HandleSceneOpening(string path, OpenSceneMode mode) => SceneOpening?.Invoke(path, mode);
	private static void HandleSceneClosed(Scene scene) => SceneClosed?.Invoke(scene);
	private static void HandleSceneClosing(Scene scene, bool removingScene) => SceneClosing?.Invoke(scene, removingScene);
	private static void HandleSceneSaved(Scene scene) => SceneSaved?.Invoke(scene);
	private static void HandleSceneSaving(Scene scene, string path) => SceneSaving?.Invoke(scene, path);
#endif

	private static void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) => SceneLoaded?.Invoke(scene, loadSceneMode);
	private static void HandleSceneUnloaded(Scene scene) => SceneUnloaded?.Invoke(scene);
	private static void HandleActiveSceneChanged(Scene scene1, Scene scene2) => ActiveSceneChanged?.Invoke(scene1, scene2);

#if UNITY_EDITOR
	static void HandlePlayModeStateChange(PlayModeStateChange stateChange)
	{
		switch (stateChange)
		{
			case PlayModeStateChange.EnteredEditMode:
			case PlayModeStateChange.ExitingPlayMode:
				cachedIsPlaying = false;
				break;
			case PlayModeStateChange.EnteredPlayMode:
			case PlayModeStateChange.ExitingEditMode:
				cachedIsPlaying = true;
				break;
		}
		lastState = stateChange;
		initialized = true;
	}
#endif
	#endregion
}
