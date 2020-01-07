using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class DataInjectorGameEventEntry {
	public StringReference key = new StringReference();
	public GameEvent eventToReplace;
	public GameEvent replacementEvent;
}
