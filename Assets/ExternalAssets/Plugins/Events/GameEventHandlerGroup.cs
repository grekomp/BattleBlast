using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameEventHandlerGroup : ISerializationCallbackReceiver
{
	protected List<GameEvent> boundEvents = new List<GameEvent>();

	[SerializeField]
	[Tooltip("This field is used to change the bound GameEvents in the inspector.")]
	protected List<GameEvent> inspectedEvents = new List<GameEvent>();

	public List<GameEvent> BoundEvents {
		get { return new List<GameEvent>(boundEvents); }
	}

	// Registered event handlers
	public List<Action> OnEventRaised = new List<Action>();
	public List<Action<GameEventData>> OnEventDataRaised = new List<Action<GameEventData>>();

	public void Raise()
	{
		foreach (var boundEvent in BoundEvents)
		{
			boundEvent?.Raise();
		}
	}
	public void Raise(object caller, object data = null)
	{
		foreach (var boundEvent in BoundEvents)
		{
			boundEvent?.Raise(caller, data);
		}
	}

	public void RegisterListenerOnce(Action handler)
	{
		DeregisterListener(handler);
		RegisterListener(handler);
	}

	public bool ReplaceEvent(GameEvent from, GameEvent to)
	{
		if (boundEvents.Contains(from))
		{
			UnbindEvent(from);
			BindEvent(to);

			return true;
		}

		return false;
	}

	public void RegisterListener(Action handler)
	{
		OnEventRaised.Add(handler);

		foreach (var boundEvent in boundEvents)
		{
			boundEvent.RegisterListener(handler);
		}
	}
	public void DeregisterListener(Action handler)
	{
		if (OnEventRaised.Contains(handler))
			OnEventRaised.Remove(handler);

		foreach (var boundEvent in boundEvents)
		{
			boundEvent.DeregisterListener(handler);
		}
	}
	public void RegisterListenerOnce(Action<GameEventData> handler)
	{
		DeregisterListener(handler);
		RegisterListener(handler);
	}
	public void RegisterListener(Action<GameEventData> handler)
	{
		OnEventDataRaised.Add(handler);

		foreach (var boundEvent in boundEvents)
		{
			boundEvent.RegisterListener(handler);
		}
	}
	public void DeregisterListener(Action<GameEventData> handler)
	{
		if (OnEventDataRaised.Contains(handler))
			OnEventDataRaised.Remove(handler);

		foreach (var boundEvent in boundEvents)
		{
			boundEvent.DeregisterListener(handler);
		}
	}

	protected void UnbindEvent(GameEvent boundEvent)
	{
		if (boundEvents.Contains(boundEvent))
		{
			foreach (var handler in OnEventRaised)
			{
				boundEvent?.DeregisterListener(handler);
			}

			foreach (var handler in OnEventDataRaised)
			{
				boundEvent?.DeregisterListener(handler);
			}
			boundEvents.Remove(boundEvent);
		}
	}
	protected void BindEvent(GameEvent newGameEvent)
	{
		if (boundEvents.Contains(newGameEvent) == false && newGameEvent != null)
		{
			boundEvents.Add(newGameEvent);
			foreach (var handler in OnEventRaised)
			{
				newGameEvent.RegisterListener(handler);
			}

			foreach (var handler in OnEventDataRaised)
			{
				newGameEvent.RegisterListener(handler);
			}
		}
	}

	protected void OnValidate()
	{
		if (!boundEvents.All(inspectedEvents.Contains) || !inspectedEvents.All(boundEvents.Contains))
		{
			// Unbind events missing from inspectedEvents
			foreach (var gameEvent in boundEvents.Except(inspectedEvents).ToList())
			{
				UnbindEvent(gameEvent);
			}

			// Bind newly added events
			foreach (var gameEvent in inspectedEvents.Except(boundEvents).ToList())
			{
				BindEvent(gameEvent);
			}
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
}
