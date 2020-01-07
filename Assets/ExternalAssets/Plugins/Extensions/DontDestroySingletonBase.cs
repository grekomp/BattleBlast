using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DontDestroySingletonBase : MonoBehaviour {

	protected static bool destroying = false;

	public virtual void InitNewInstance() { }

	void OnDestroy() {
		destroying = true;
	}
}
