using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	public event Action OnEventRaised;
	public event Action<GameEventData> OnEventDataRaised;

	public bool debug = false;

	public static GameEvent New()
	{
		return CreateInstance<GameEvent>();
	}
	public static GameEvent New(Action handler)
	{
		return New().RegisterListener(handler);
	}
	public static GameEvent New(Action<GameEventData> handler)
	{
		return New().RegisterListener(handler);
	}

	public void Raise()
	{
		if (debug)
		{
			Debug.Log(string.Format("GameEvent Raised ({0}), no data passed.", name), this);
		}

		try
		{
			OnEventRaised?.Invoke();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}
	public void Raise(object caller, object data = null)
	{
		if (debug)
		{
			Debug.Log(string.Format("GameEvent Raised ({0}), caller: {1}, data: {2}", name, caller, data), this);
		}

		try
		{
			OnEventRaised?.Invoke();
			OnEventDataRaised?.Invoke(new GameEventData(this, data, caller));
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}

	public GameEvent RegisterListener(Action handler)
	{
		OnEventRaised += handler;
		return this;
	}
	public GameEvent DeregisterListener(Action handler)
	{
		OnEventRaised -= handler;
		return this;
	}
	public GameEvent RegisterListener(Action<GameEventData> handler)
	{
		OnEventDataRaised += handler;
		return this;
	}
	public GameEvent DeregisterListener(Action<GameEventData> handler)
	{
		OnEventDataRaised -= handler;
		return this;
	}

#if UNITY_EDITOR
	public Delegate[] GetOnEventRaisedInvocationsList()
	{
		return OnEventRaised?.GetInvocationList();
	}
	public Delegate[] GetOnEventDataRaisedInvocationsList()
	{
		return OnEventDataRaised?.GetInvocationList();
	}
#endif
}
