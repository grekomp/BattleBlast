using System;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class GameEventListener : DataInjectionReceiver
{
	[Serializable]
	public class GameEventDataUnityEvent : UnityEvent<GameEventData> { }

	public GameEventHandler gameEvent = new GameEventHandler();
	public bool enableInEditor = true;
	public bool debug = false;

	[Space]
	public UnityEvent response = new UnityEvent();
	public GameEventDataUnityEvent responseWithData = new GameEventDataUnityEvent();

	private void OnEnable()
	{
		gameEvent.RegisterListener(OnEventRaised);
		gameEvent.RegisterListener(OnEventDataRaised);
	}
	private void OnDisable()
	{
		gameEvent.DeregisterListener(OnEventRaised);
		gameEvent.DeregisterListener(OnEventDataRaised);
	}

	[ContextMenu("OnEventRaised")]
	public void OnEventRaised()
	{
		if (debug) Debug.Log(this + "(GameEventListener): OnEventRaised", this);
		response.Invoke();
	}
	public void OnEventDataRaised(GameEventData data)
	{
		if (debug) Debug.Log(string.Format("{0}(GameEventListener): OnEventDataRaised({1})", this, data), this);
		responseWithData.Invoke(data);
	}

	[ContextMenu("Update Editor Call State")]
	private void UpdateResponsesCallState()
	{
		for (int i = 0; i < response.GetPersistentEventCount(); i++)
		{
			response.SetPersistentListenerState(i, enableInEditor ? UnityEventCallState.EditorAndRuntime : UnityEventCallState.RuntimeOnly);
		}

		for (int i = 0; i < responseWithData.GetPersistentEventCount(); i++)
		{
			responseWithData.SetPersistentListenerState(i, enableInEditor ? UnityEventCallState.EditorAndRuntime : UnityEventCallState.RuntimeOnly);
		}
	}
}