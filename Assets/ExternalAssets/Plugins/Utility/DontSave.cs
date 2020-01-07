#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class DontSave : MonoBehaviour {
	private void Awake() {
		gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
		foreach (Transform child in GetComponentsInChildren<Transform>(true)) {
			child.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
		}
	}

	public void Dispose() {
		gameObject.DestroyAnywhere();
	}

#if UNITY_EDITOR
	private void OnEnable() {
		EditorSceneManager.sceneOpened += HandleSceneOpened;
		EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
	}
	private void OnDisable() {
		EditorSceneManager.sceneOpened -= HandleSceneOpened;
		EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
	}

	private void HandlePlayModeStateChange(PlayModeStateChange stateChange) => Dispose();
	private void HandleSceneOpened(Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode) => Dispose();
#endif
}
