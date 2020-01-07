using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableVariableManager : MonoBehaviour {
	//#region Singleton
	//public static ScriptableVariableManager instance;
	//#endregion

	public ScriptableVariableSet variableSet;

	private void Awake() {
		//if (instance == null)
		//{
		//	instance = this;

		//	DontDestroyOnLoad(gameObject);

		variableSet.Awake();
		//}
		//else
		//{
		//	Destroy(gameObject);
		//}
	}
}
