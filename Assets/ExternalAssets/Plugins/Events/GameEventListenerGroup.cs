using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class GameEventListenerGroup : DataInjectionReceiver {
	public GameEventHandlerGroup gameEvents = new GameEventHandlerGroup();
	public bool enableInEditor = true;

	public UnityEvent response = new UnityEvent();
	public GameEventListener.GameEventDataUnityEvent responseWithData = new GameEventListener.GameEventDataUnityEvent();

	private void OnEnable() {
		gameEvents.RegisterListener(OnEventRaised);
		gameEvents.RegisterListener(OnEventDataRaised);
	}
	private void OnDisable() {
		gameEvents.DeregisterListener(OnEventRaised);
		gameEvents.DeregisterListener(OnEventDataRaised);
	}

	[ContextMenu("OnEventRaised")]
	public void OnEventRaised() {
		response.Invoke();
	}
	public void OnEventDataRaised(GameEventData data) {
		responseWithData.Invoke(data);
	}

	[ContextMenu("Update Editor Call State")]
	private void UpdateResponsesCallState() {
		for (int i = 0; i < response.GetPersistentEventCount(); i++) {
			response.SetPersistentListenerState(i, enableInEditor ? UnityEventCallState.EditorAndRuntime : UnityEventCallState.RuntimeOnly);
		}

		for (int i = 0; i < responseWithData.GetPersistentEventCount(); i++) {
			responseWithData.SetPersistentListenerState(i, enableInEditor ? UnityEventCallState.EditorAndRuntime : UnityEventCallState.RuntimeOnly);
		}
	}
}
