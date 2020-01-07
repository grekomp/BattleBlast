using UnityEngine;
using System.Collections;

namespace ScriptableSystems {
	public class ScriptableSystemsEventHelper : DontDestroySingleton<ScriptableSystemsEventHelper> {
		private void Awake() {
			ScriptableSystemsManager.Instance.Awake();
		}
		private void Update() {
			ScriptableSystemsManager.Instance.Update();
		}
	}
}