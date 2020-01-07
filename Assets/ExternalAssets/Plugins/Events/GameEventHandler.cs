using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class GameEventHandler : ISerializationCallbackReceiver
{
	protected GameEvent boundEvent = null;

	[SerializeField]
	[Tooltip("This field is used to change the bound GameEvent in the inspector.")]
	protected GameEvent inspectedEvent = null;
	public GameEvent BoundEvent {
		get {
			return boundEvent;
		}
		set {
			BindEvent(value);
		}
	}

	// Registered event handlers
	public List<Action> OnEventRaised = new List<Action>();
	public List<Action<GameEventData>> OnEventDataRaised = new List<Action<GameEventData>>();

	#region Raising Event
	public void Raise()
	{
		if (BoundEvent != null)
		{
			BoundEvent.Raise();
		}
		else
		{
			RaiseRegisteredHandlers();
		}
	}
	public void Raise(object caller, object data = null)
	{
		if (BoundEvent != null)
		{
			BoundEvent.Raise(caller, data);
		}
		else
		{
			RaiseRegisteredHandlers();
			RaiseRegisteredDataHandlers(caller, data);
		}
	}

	protected void RaiseRegisteredHandlers()
	{
		List<Action> OnEventRaisedCopy = new List<Action>(OnEventRaised);

		foreach (var listener in OnEventRaisedCopy)
		{
			listener?.Invoke();
		}
	}
	protected void RaiseRegisteredDataHandlers(object caller, object data)
	{
		List<Action<GameEventData>> OnEventDataRaisedCopy = new List<Action<GameEventData>>(OnEventDataRaised);

		foreach (var listener in OnEventDataRaisedCopy)
		{
			listener?.Invoke(new GameEventData(null, data, caller));
		}
	}
	#endregion

	#region Registering Listeners
	public void RegisterListenerOnce(Action handler)
	{
		DeregisterListener(handler);
		RegisterListener(handler);
	}
	public void RegisterListener(Action handler)
	{
		OnEventRaised.Add(handler);

		if (boundEvent)
			boundEvent.RegisterListener(handler);
	}
	public void DeregisterListener(Action handler)
	{
		if (OnEventRaised.Contains(handler))
			OnEventRaised.Remove(handler);

		if (boundEvent)
			boundEvent.DeregisterListener(handler);
	}
	public void RegisterListenerOnce(Action<GameEventData> handler)
	{
		DeregisterListener(handler);
		RegisterListener(handler);
	}
	public void RegisterListener(Action<GameEventData> handler)
	{
		OnEventDataRaised.Add(handler);

		if (boundEvent)
			boundEvent.RegisterListener(handler);
	}
	public void DeregisterListener(Action<GameEventData> handler)
	{
		if (OnEventDataRaised.Contains(handler))
			OnEventDataRaised.Remove(handler);

		if (boundEvent)
			boundEvent.DeregisterListener(handler);
	}
	#endregion

	#region Binding Events
	protected void UnbindCurrentEvent()
	{
		if (boundEvent)
		{
			foreach (var handler in OnEventRaised)
			{
				boundEvent.DeregisterListener(handler);
			}

			foreach (var handler in OnEventDataRaised)
			{
				boundEvent.DeregisterListener(handler);
			}
		}

		boundEvent = null;
		inspectedEvent = boundEvent;
	}
	protected void BindEvent(GameEvent newGameEvent)
	{
		if (boundEvent) UnbindCurrentEvent();

		boundEvent = newGameEvent;
		if (boundEvent)
		{
			foreach (var handler in OnEventRaised)
			{
				boundEvent.RegisterListener(handler);
			}

			foreach (var handler in OnEventDataRaised)
			{
				boundEvent.RegisterListener(handler);
			}
		}
		inspectedEvent = boundEvent;
	}

	public bool ReplaceEvent(GameEvent from, GameEvent to)
	{
		if (BoundEvent == from)
		{
			BoundEvent = to;
			return true;
		}

		return false;
	}
	#endregion

	#region Editor Helpers
	protected void OnValidate()
	{
		if (inspectedEvent != boundEvent)
		{
			BoundEvent = inspectedEvent;
		}
	}
	public void OnBeforeSerialize()
	{
		OnValidate();
	}
	public void OnAfterDeserialize()
	{
		OnValidate();
	}
	#endregion
}
